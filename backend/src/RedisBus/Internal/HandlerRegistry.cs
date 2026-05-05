using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RedisBus.Internal;

internal delegate Task<object?> Dispatch(object message, MessageContext context, IServiceProvider services, CancellationToken cancellationToken);

internal sealed class HandlerRegistry
{
    public sealed record Invoker(Type MessageType, Type? ResponseType, Dispatch Dispatch)
    {
        public bool IsResponder => ResponseType is not null;
    }

    private readonly Dictionary<string, Invoker> _byChannel = new();

    public HandlerRegistry(IOptions<RedisBusOptions> options, ChannelNamer namer)
    {
        foreach (var assembly in options.Value.HandlerAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface) continue;

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;
                    var def = iface.GetGenericTypeDefinition();

                    if (def == typeof(IHandle<>))
                    {
                        var msgType = iface.GetGenericArguments()[0];
                        _byChannel[namer.ForMessage(msgType)] =
                            new Invoker(msgType, null, BuildDispatch(nameof(DispatchHandler), msgType));
                    }
                    else if (def == typeof(IRespond<,>))
                    {
                        var args = iface.GetGenericArguments();
                        _byChannel[namer.ForMessage(args[0])] =
                            new Invoker(args[0], args[1], BuildDispatch(nameof(DispatchResponder), args[0], args[1]));
                    }
                }
            }
        }
    }

    public IReadOnlyCollection<string> Channels => _byChannel.Keys;

    public bool TryGet(string channel, [MaybeNullWhen(false)] out Invoker invoker)
        => _byChannel.TryGetValue(channel, out invoker);

    private static Dispatch BuildDispatch(string methodName, params Type[] genericArgs)
    {
        var generic = typeof(HandlerRegistry).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;
        return generic.MakeGenericMethod(genericArgs).CreateDelegate<Dispatch>();
    }

    private static async Task<object?> DispatchHandler<TMsg>(
        object message, MessageContext ctx, IServiceProvider sp, CancellationToken ct)
        where TMsg : IMessage
    {
        await sp.GetRequiredService<IHandle<TMsg>>().HandleAsync((TMsg)message, ctx, ct);
        return null;
    }

    private static async Task<object?> DispatchResponder<TMsg, TResp>(
        object message, MessageContext ctx, IServiceProvider sp, CancellationToken ct)
        where TMsg : IMessage
    {
        return await sp.GetRequiredService<IRespond<TMsg, TResp>>().RespondAsync((TMsg)message, ctx, ct);
    }
}
