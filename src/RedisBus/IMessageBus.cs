namespace RedisBus;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;

    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
