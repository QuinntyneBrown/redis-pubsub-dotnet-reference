namespace RedisBus;

public sealed class MessageValidationException : Exception
{
    public Type MessageType { get; }
    public IReadOnlyList<string> Errors { get; }

    public MessageValidationException(Type messageType, IReadOnlyList<string> errors)
        : base($"Validation failed for {messageType.Name}: {string.Join("; ", errors)}")
    {
        MessageType = messageType;
        Errors = errors;
    }
}
