using System.Text.Json;

namespace RedisBus.Internal;

internal sealed class EnvelopeSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public string Serialize(
        Guid messageId,
        Guid correlationId,
        string? replyTo,
        Type payloadType,
        DateTimeOffset occurredAt,
        string publisher,
        object payload)
    {
        var envelope = new MessageEnvelope
        {
            MessageId = messageId,
            CorrelationId = correlationId,
            ReplyTo = replyTo,
            Type = $"{payloadType.FullName}, {payloadType.Assembly.GetName().Name}",
            OccurredAt = occurredAt,
            Publisher = publisher,
            Payload = JsonSerializer.SerializeToElement(payload, payloadType, Options),
        };
        return JsonSerializer.Serialize(envelope, Options);
    }

    public MessageEnvelope Deserialize(string json)
        => JsonSerializer.Deserialize<MessageEnvelope>(json, Options)
           ?? throw new InvalidOperationException("Envelope deserialized to null.");

    public object DeserializePayload(JsonElement payload, Type targetType)
        => payload.Deserialize(targetType, Options)
           ?? throw new InvalidOperationException($"Payload deserialized to null for type {targetType.FullName}.");
}
