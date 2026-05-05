using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using RedisBus.Internal;
using RedisBus.UnitTests.SampleMessages;
using Xunit;

namespace RedisBus.UnitTests;

public class PipelineTests
{
    private readonly Pipeline _pipeline = new(NullLogger<Pipeline>.Instance);

    [Fact]
    public async Task Send_with_valid_message_invokes_publisher()
    {
        var msg = new TempReading("S-1", 22.5);
        var published = false;

        await _pipeline.RunSendAsync(msg, "tel.SampleMessages.TempReading", Guid.NewGuid(), Guid.NewGuid(), () =>
        {
            published = true;
            return Task.CompletedTask;
        });

        published.Should().BeTrue();
    }

    [Fact]
    public async Task Send_with_invalid_message_throws_MessageValidationException()
    {
        var msg = new TempReading("", -100);
        var act = () => _pipeline.RunSendAsync(msg, "tel.SampleMessages.TempReading", Guid.NewGuid(), Guid.NewGuid(), () => Task.CompletedTask);
        await act.Should().ThrowAsync<MessageValidationException>();
    }

    [Fact]
    public async Task Receive_one_way_with_invalid_message_skips_handler()
    {
        var msg = new TempReading("", 200);
        var ctx = new MessageContext(Guid.NewGuid(), Guid.NewGuid(), "test", DateTimeOffset.UtcNow);
        var invoked = false;

        await _pipeline.RunReceiveOneWayAsync(msg, "tel.SampleMessages.TempReading", ctx, () =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task Receive_one_way_swallows_handler_exceptions()
    {
        var msg = new TempReading("S-1", 22.5);
        var ctx = new MessageContext(Guid.NewGuid(), Guid.NewGuid(), "test", DateTimeOffset.UtcNow);

        var act = () => _pipeline.RunReceiveOneWayAsync(msg, "tel.SampleMessages.TempReading", ctx,
            () => throw new InvalidOperationException("boom"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Receive_responder_with_invalid_message_returns_null()
    {
        var msg = new SetValue("", 200);
        var ctx = new MessageContext(Guid.NewGuid(), Guid.NewGuid(), "test", DateTimeOffset.UtcNow);

        var result = await _pipeline.RunReceiveResponderAsync(msg, "req.SampleMessages.SetValue", ctx,
            () => Task.FromResult<object?>(new SetValueAck("k", 1)));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Receive_responder_returns_value_on_success()
    {
        var msg = new SetValue("k", 50);
        var ctx = new MessageContext(Guid.NewGuid(), Guid.NewGuid(), "test", DateTimeOffset.UtcNow);
        var ack = new SetValueAck("k", 50);

        var result = await _pipeline.RunReceiveResponderAsync(msg, "req.SampleMessages.SetValue", ctx,
            () => Task.FromResult<object?>(ack));

        result.Should().Be(ack);
    }
}
