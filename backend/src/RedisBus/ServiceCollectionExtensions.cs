using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedisBus.Internal;
using StackExchange.Redis;

namespace RedisBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisBus(this IServiceCollection services, Action<RedisBusOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisBusOptions>>().Value;
            return ConnectionMultiplexer.Connect(options.ConnectionString);
        });

        services.AddSingleton<ChannelNamer>();
        services.AddSingleton<EnvelopeSerializer>();
        services.AddSingleton<PendingReplies>();
        services.AddSingleton<Pipeline>();
        services.AddSingleton<HandlerRegistry>();
        services.AddSingleton<IMessageBus, MessageBus>();
        services.AddHostedService<BusHostedService>();

        return services;
    }

    public static IServiceCollection AddRedisBusHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.Configure<RedisBusOptions>(o =>
        {
            if (!o.HandlerAssemblies.Contains(assembly))
                o.HandlerAssemblies.Add(assembly);
        });

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType) continue;
                var def = iface.GetGenericTypeDefinition();
                if (def == typeof(IHandle<>) || def == typeof(IRespond<,>))
                    services.AddScoped(iface, type);
            }
        }

        return services;
    }
}
