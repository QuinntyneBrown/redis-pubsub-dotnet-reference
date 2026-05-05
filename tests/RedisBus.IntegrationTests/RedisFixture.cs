using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;
using Testcontainers.Redis;
using Xunit;

namespace RedisBus.IntegrationTests;

public sealed class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _container = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public async Task<IHost> StartServiceAsync(string serviceName, Assembly handlerAssembly)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddRedisBus(o =>
                {
                    o.ConnectionString = ConnectionString;
                    o.ServiceName = serviceName;
                });
                services.AddRedisBusHandlers(handlerAssembly);
            })
            .Build();

        await host.StartAsync();
        return host;
    }
}

[CollectionDefinition(Name)]
public sealed class RedisCollection : ICollectionFixture<RedisFixture>
{
    public const string Name = "redis";
}
