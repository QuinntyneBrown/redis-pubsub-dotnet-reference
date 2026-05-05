namespace RedisBus;

public interface IHandle<in T> where T : IMessage
{
    Task HandleAsync(T message, MessageContext context, CancellationToken cancellationToken);
}
