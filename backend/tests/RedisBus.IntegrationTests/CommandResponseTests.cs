using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RedisBus.IntegrationTests.TestMessages;
using Xunit;

namespace RedisBus.IntegrationTests;

[Collection(RedisCollection.Name)]
public class CommandResponseTests
{
    private readonly RedisFixture _redis;

    public CommandResponseTests(RedisFixture redis) => _redis = redis;

    [Fact]
    public async Task Command_returns_ack_from_responder()
    {
        using var responder = await _redis.StartServiceAsync("responder-cmd", typeof(SampleCommandResponder).Assembly);
        using var issuer = await _redis.StartServiceAsync("issuer-cmd", typeof(SampleCommandResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var ack = await bus.SendAsync(new SampleCommand("default"));

        ack.Profile.Should().Be("default");
        ack.SessionId.Should().Be("session-default");
    }

    [Fact]
    public async Task Command_with_invalid_payload_throws_MessageValidationException()
    {
        using var issuer = await _redis.StartServiceAsync("issuer-cmd-invalid", typeof(SampleCommandResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var act = () => bus.SendAsync(new SampleCommand(""));

        await act.Should().ThrowAsync<MessageValidationException>();
    }
}
