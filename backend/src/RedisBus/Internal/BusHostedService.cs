using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace RedisBus.Internal;

internal sealed class BusHostedService : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly HandlerRegistry _registry;
    private readonly EnvelopeSerializer _serializer;
    private readonly Pipeline _pipeline;
    private readonly PendingReplies _pendingReplies;
    private readonly RedisBusOptions _options;
    private readonly ChannelNamer _channelNamer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BusHostedService> _logger;

    private ISubscriber? _subscriber;

    public BusHostedService(
        IConnectionMultiplexer redis,
        HandlerRegistry registry,
        EnvelopeSerializer serializer,
        Pipeline pipeline,
        PendingReplies pendingReplies,
        IOptions<RedisBusOptions> options,
        ChannelNamer channelNamer,
        IServiceScopeFactory scopeFactory,
        ILogger<BusHostedService> logger)
    {
        _redis = redis;
        _registry = registry;
        _serializer = serializer;
        _pipeline = pipeline;
        _pendingReplies = pendingReplies;
        _options = options.Value;
        _channelNamer = channelNamer;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _subscriber = _redis.GetSubscriber();

        var replyChannel = _channelNamer.ForReply(_options.ServiceName, _options.InstanceId);
        var replyQueue = await _subscriber.SubscribeAsync(RedisChannel.Literal(replyChannel));
        replyQueue.OnMessage(msg => OnReply(msg.Message.ToString()));

        foreach (var channel in _registry.Channels)
        {
            var queue = await _subscriber.SubscribeAsync(RedisChannel.Literal(channel));
            var captured = channel;
            queue.OnMessage(msg => OnDispatchAsync(captured, msg.Message.ToString()));
        }

        _logger.LogInformation(
            "RedisBus started for service {Service} (instance {Instance}); subscribed to {Count} handler channels + reply channel.",
            _options.ServiceName, _options.InstanceId, _registry.Channels.Count);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_subscriber is not null) await _subscriber.UnsubscribeAllAsync();
    }

    private void OnReply(string raw)
    {
        try
        {
            var envelope = _serializer.Deserialize(raw);
            if (!_pendingReplies.TryComplete(envelope.CorrelationId, envelope.Payload))
                _logger.LogWarning("Late reply for correlation id {CorrelationId}; dropped.", envelope.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to process reply envelope.");
        }
    }

    private async Task OnDispatchAsync(string channel, string raw)
    {
        try
        {
            var envelope = _serializer.Deserialize(raw);

            if (!_registry.TryGet(channel, out var invoker))
            {
                _logger.LogWarning("No handler registered for channel {Channel}.", channel);
                return;
            }

            var clrType = Type.GetType(envelope.Type);
            if (clrType is null)
            {
                _logger.LogWarning("Cannot resolve CLR type {Type} for {Channel}; dropped.", envelope.Type, channel);
                return;
            }

            var payload = _serializer.DeserializePayload(envelope.Payload, clrType);
            var ctx = new MessageContext(envelope.MessageId, envelope.CorrelationId, envelope.Publisher, envelope.OccurredAt);

            using var scope = _scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            if (!invoker.IsResponder)
            {
                await _pipeline.RunReceiveOneWayAsync(payload, channel, ctx,
                    () => invoker.Dispatch(payload, ctx, sp, CancellationToken.None));
                return;
            }

            var result = await _pipeline.RunReceiveResponderAsync(payload, channel, ctx,
                () => invoker.Dispatch(payload, ctx, sp, CancellationToken.None));

            if (result is null || envelope.ReplyTo is null) return;

            var replyJson = _serializer.Serialize(
                messageId: Guid.NewGuid(),
                correlationId: envelope.CorrelationId,
                replyTo: null,
                payloadType: result.GetType(),
                occurredAt: DateTimeOffset.UtcNow,
                publisher: _options.ServiceName,
                payload: result);

            await _subscriber!.PublishAsync(RedisChannel.Literal(envelope.ReplyTo), replyJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error dispatching message on {Channel}.", channel);
        }
    }
}
