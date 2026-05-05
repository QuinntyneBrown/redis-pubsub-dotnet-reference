using System.Collections.Concurrent;

namespace BffService.Subscriptions;

public sealed class SubscriptionRegistry
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _byConnection = new();

    public void Add(string connectionId, string channel)
    {
        var channels = _byConnection.GetOrAdd(connectionId, _ => new ConcurrentDictionary<string, byte>());
        channels[channel] = 0;
    }

    public void Remove(string connectionId, string channel)
    {
        if (_byConnection.TryGetValue(connectionId, out var channels))
            channels.TryRemove(channel, out _);
    }

    public void Clear(string connectionId)
        => _byConnection.TryRemove(connectionId, out _);

    public IEnumerable<string> ConnectionsFor(string channel)
    {
        foreach (var (connectionId, channels) in _byConnection)
            if (channels.ContainsKey(channel))
                yield return connectionId;
    }
}
