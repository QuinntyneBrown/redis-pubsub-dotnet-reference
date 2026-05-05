using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace RedisBus.Internal;

internal sealed class MessageBus : IMessageBus
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ChannelNamer _channelNamer;
    private readonly EnvelopeSerializer _serializer;
    private readonly PendingReplies _pendingReplies;
    private readonly Pipeline _pipeline;
    private readonly RedisBusOptions _options;
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(
        IConnectionMultiplexer redis,
        ChannelNamer channelNamer,
        EnvelopeSerializer serializer,
        PendingReplies pendingReplies,
        Pipeline pipeline,
        IOptions<RedisBusOptions> options,
        ILogger<MessageBus> logger)
    {
        _redis = redis;
        _channelNamer = channelNamer;
        _serializer = serializer;
        _pendingReplies = pendingReplies;
        _pipeline = pipeline;
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        var channel = _channelNamer.ForMessage(message.GetType());
        var messageId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        return _pipeline.RunSendAsync(message, channel, messageId, correlationId,
            () => PublishEnvelope(channel, messageId, correlationId, replyTo: null, message));
    }

    public Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => SendInternal<TResponse>(query, timeout, cancellationToken);

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => SendInternal<TResponse>(request, timeout, cancellationToken);

    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => SendInternal<TResponse>(command, timeout, cancellationToken);

    private async Task<TResponse> SendInternal<TResponse>(IMessage message, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        var channel = _channelNamer.ForMessage(message.GetType());
        var messageId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var replyTo = _channelNamer.ForReply(_options.ServiceName, _options.InstanceId);

        var waiter = _pendingReplies.Register(correlationId, timeout ?? _options.DefaultTimeout, cancellationToken);

        await _pipeline.RunSendAsync(message, channel, messageId, correlationId,
            () => PublishEnvelope(channel, messageId, correlationId, replyTo, message));

        var payload = await waiter;
        return (TResponse)_serializer.DeserializePayload(payload, typeof(TResponse));
    }

    private async Task PublishEnvelope(string channel, Guid messageId, Guid correlationId, string? replyTo, object payload)
    {
        var json = _serializer.Serialize(
            messageId: messageId,
            correlationId: correlationId,
            replyTo: replyTo,
            payloadType: payload.GetType(),
            occurredAt: DateTimeOffset.UtcNow,
            publisher: _options.ServiceName,
            payload: payload);

        await _redis.GetSubscriber().PublishAsync(RedisChannel.Literal(channel), json);
    }
}
