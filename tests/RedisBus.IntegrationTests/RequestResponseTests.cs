using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus.IntegrationTests.TestMessages;
using Xunit;

namespace RedisBus.IntegrationTests;

[Collection(RedisCollection.Name)]
public class RequestResponseTests
{
    private readonly RedisFixture _redis;

    public RequestResponseTests(RedisFixture redis) => _redis = redis;

    [Fact]
    public async Task Request_returns_ack_from_responder()
    {
        using var responder = await _redis.StartServiceAsync("responder-req", typeof(SampleRequestResponder).Assembly);
        using var issuer = await _redis.StartServiceAsync("issuer-req", typeof(SampleRequestResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var ack = await bus.SendAsync(new SampleRequest("device-1", 42));

        ack.Key.Should().Be("device-1");
        ack.Applied.Should().Be(42);
    }

    [Fact]
    public async Task Request_with_invalid_payload_throws_MessageValidationException()
    {
        using var issuer = await _redis.StartServiceAsync("issuer-invalid", typeof(SampleRequestResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var act = () => bus.SendAsync(new SampleRequest("", 999));

        await act.Should().ThrowAsync<MessageValidationException>();
    }
}
