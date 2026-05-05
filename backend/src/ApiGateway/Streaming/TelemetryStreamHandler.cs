using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ApiGateway.Streaming;

public sealed class TelemetryStreamHandler : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly SseBroker _broker;
    private readonly ILogger<TelemetryStreamHandler> _logger;
    private ChannelMessageQueue? _queue;

    public TelemetryStreamHandler(IConnectionMultiplexer redis, SseBroker broker, ILogger<TelemetryStreamHandler> logger)
    {
        _redis = redis;
        _broker = broker;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _queue = await _redis.GetSubscriber().SubscribeAsync(RedisChannel.Pattern("tel.*"));
        _queue.OnMessage(msg => _broker.Broadcast(SseBroker.Telemetry, msg.Message.ToString()));
        _logger.LogInformation("Forwarding tel.* envelopes to SSE telemetry topic.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_queue is not null) await _queue.UnsubscribeAsync();
    }
}
