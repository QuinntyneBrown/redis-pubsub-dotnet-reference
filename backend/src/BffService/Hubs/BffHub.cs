using System.Text.Json;
using BffService.Subscriptions;
using Microsoft.AspNetCore.SignalR;

namespace BffService.Hubs;

public sealed class BffHub : Hub
{
    private readonly MessageBusBridge _bridge;
    private readonly SubscriptionRegistry _subscriptions;

    public BffHub(MessageBusBridge bridge, SubscriptionRegistry subscriptions)
    {
        _bridge = bridge;
        _subscriptions = subscriptions;
    }

    public Task<object?> Send(string typeName, JsonElement payload, CancellationToken cancellationToken)
        => _bridge.DispatchAsync(typeName, payload, cancellationToken);

    public void Subscribe(string channel)
        => _subscriptions.Add(Context.ConnectionId, channel);

    public void Unsubscribe(string channel)
        => _subscriptions.Remove(Context.ConnectionId, channel);

    public void UnsubscribeAll()
        => _subscriptions.Clear(Context.ConnectionId);

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _subscriptions.Clear(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
