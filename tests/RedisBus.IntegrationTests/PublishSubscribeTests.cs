using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus.IntegrationTests.TestMessages;
using Xunit;

namespace RedisBus.IntegrationTests;

[Collection(RedisCollection.Name)]
public class PublishSubscribeTests
{
    private readonly RedisFixture _redis;

    public PublishSubscribeTests(RedisFixture redis) => _redis = redis;

    [Fact]
    public async Task Telemetry_published_by_one_service_is_received_by_subscriber()
    {
        Inbox.Clear();
        using var subscriber = await _redis.StartServiceAsync("subscriber-tel", typeof(SampleTelemetryHandler).Assembly);
        using var publisher = await _redis.StartServiceAsync("publisher-tel", typeof(SampleTelemetryHandler).Assembly);
        var bus = publisher.Services.GetRequiredService<IMessageBus>();

        await bus.PublishAsync(new SampleTelemetry("S-1", 21.5));

        await WaitForReceived<SampleTelemetry>(timeout: TimeSpan.FromSeconds(3));
        Inbox.Received.OfType<SampleTelemetry>().Should().Contain(t => t.SensorId == "S-1");
    }

    [Fact]
    public async Task Event_fans_out_to_every_subscriber()
    {
        Inbox.Clear();
        using var sub1 = await _redis.StartServiceAsync("sub1-evt", typeof(SampleEventHandler).Assembly);
        using var sub2 = await _redis.StartServiceAsync("sub2-evt", typeof(SampleEventHandler).Assembly);
        using var publisher = await _redis.StartServiceAsync("pub-evt", typeof(SampleEventHandler).Assembly);
        var bus = publisher.Services.GetRequiredService<IMessageBus>();

        await bus.PublishAsync(new SampleEvent("RoomA", "occupied"));

        await WaitForCount<SampleEvent>(expected: 3, timeout: TimeSpan.FromSeconds(3));
        Inbox.Received.OfType<SampleEvent>().Should().HaveCount(3);
    }

    private static async Task WaitForReceived<T>(TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (Inbox.Received.OfType<T>().Any()) return;
            await Task.Delay(25);
        }
    }

    private static async Task WaitForCount<T>(int expected, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (Inbox.Received.OfType<T>().Count() >= expected) return;
            await Task.Delay(25);
        }
    }
}
