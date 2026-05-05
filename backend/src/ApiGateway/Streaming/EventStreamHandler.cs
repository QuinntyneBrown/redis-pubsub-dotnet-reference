using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ApiGateway.Streaming;

public sealed class EventStreamHandler : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly SseBroker _broker;
    private readonly ILogger<EventStreamHandler> _logger;
    private ChannelMessageQueue? _queue;

    public EventStreamHandler(IConnectionMultiplexer redis, SseBroker broker, ILogger<EventStreamHandler> logger)
    {
        _redis = redis;
        _broker = broker;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _queue = await _redis.GetSubscriber().SubscribeAsync(RedisChannel.Pattern("evt.*"));
        _queue.OnMessage(msg => _broker.Broadcast(SseBroker.Events, msg.Message.ToString()));
        _logger.LogInformation("Forwarding evt.* envelopes to SSE events topic.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_queue is not null) await _queue.UnsubscribeAsync();
    }
}
