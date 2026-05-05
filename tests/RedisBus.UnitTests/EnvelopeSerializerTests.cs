using FluentAssertions;
using RedisBus.Internal;
using RedisBus.UnitTests.SampleMessages;
using Xunit;

namespace RedisBus.UnitTests;

public class EnvelopeSerializerTests
{
    private readonly EnvelopeSerializer _serializer = new();

    [Fact]
    public void Round_trips_payload_through_envelope()
    {
        var original = new TempReading("S-1", 22.5);
        var json = _serializer.Serialize(
            messageId: Guid.NewGuid(),
            correlationId: Guid.NewGuid(),
            replyTo: null,
            payloadType: typeof(TempReading),
            occurredAt: DateTimeOffset.UtcNow,
            publisher: "test",
            payload: original);

        var envelope = _serializer.Deserialize(json);
        var roundTripped = (TempReading)_serializer.DeserializePayload(envelope.Payload, typeof(TempReading));

        roundTripped.Should().Be(original);
    }

    [Fact]
    public void Persists_envelope_metadata()
    {
        var messageId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.Parse("2026-05-04T14:22:31.117Z");

        var json = _serializer.Serialize(
            messageId: messageId,
            correlationId: correlationId,
            replyTo: "rep.svc.abc",
            payloadType: typeof(TempReading),
            occurredAt: occurredAt,
            publisher: "sensor",
            payload: new TempReading("S-1", 22.5));

        var envelope = _serializer.Deserialize(json);

        envelope.MessageId.Should().Be(messageId);
        envelope.CorrelationId.Should().Be(correlationId);
        envelope.ReplyTo.Should().Be("rep.svc.abc");
        envelope.OccurredAt.Should().Be(occurredAt);
        envelope.Publisher.Should().Be("sensor");
        envelope.Type.Should().StartWith(typeof(TempReading).FullName);
    }
}
