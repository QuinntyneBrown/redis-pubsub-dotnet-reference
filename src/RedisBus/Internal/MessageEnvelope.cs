using System.Text.Json;

namespace RedisBus.Internal;

internal sealed class MessageEnvelope
{
    public Guid MessageId { get; set; }
    public Guid CorrelationId { get; set; }
    public string? ReplyTo { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public JsonElement Payload { get; set; }
}
