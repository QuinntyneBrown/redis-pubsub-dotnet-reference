namespace RedisBus;

public sealed record MessageContext(
    Guid MessageId,
    Guid CorrelationId,
    string Publisher,
    DateTimeOffset OccurredAt);
