using BffService.Hubs;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace BffService.Subscriptions;

public sealed class BusListener : IHostedService
{
    private static readonly string[] Patterns = { "tel.*", "evt.*" };

    private readonly IConnectionMultiplexer _redis;
    private readonly SubscriptionRegistry _subscriptions;
    private readonly IHubContext<BffHub> _hub;
    private readonly ILogger<BusListener> _logger;
    private readonly List<ChannelMessageQueue> _queues = new();

    public BusListener(
        IConnectionMultiplexer redis,
        SubscriptionRegistry subscriptions,
        IHubContext<BffHub> hub,
        ILogger<BusListener> logger)
    {
        _redis = redis;
        _subscriptions = subscriptions;
        _hub = hub;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var subscriber = _redis.GetSubscriber();
        foreach (var pattern in Patterns)
        {
            var queue = await subscriber.SubscribeAsync(RedisChannel.Pattern(pattern));
            queue.OnMessage(msg => Forward(msg.Channel.ToString(), msg.Message.ToString()));
            _queues.Add(queue);
        }
        _logger.LogInformation("BFF listening on patterns: {Patterns}", string.Join(", ", Patterns));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var queue in _queues) await queue.UnsubscribeAsync();
    }

    private void Forward(string channel, string envelopeJson)
    {
        foreach (var connectionId in _subscriptions.ConnectionsFor(channel))
            _ = _hub.Clients.Client(connectionId).SendAsync("OnMessage", channel, envelopeJson);
    }
}
