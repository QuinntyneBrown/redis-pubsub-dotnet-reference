namespace RedisBus;

public interface IRespond<in TMessage, TResponse> where TMessage : IMessage
{
    Task<TResponse> RespondAsync(TMessage message, MessageContext context, CancellationToken cancellationToken);
}
