using System.Collections.Concurrent;
using System.Threading.Channels;

namespace ApiGateway.Streaming;

public sealed class SseBroker
{
    public const string Telemetry = "telemetry";
    public const string Events = "events";

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Channel<string>>> _topics = new();

    public Guid Subscribe(string topic, out ChannelReader<string> reader)
    {
        var subs = _topics.GetOrAdd(topic, _ => new ConcurrentDictionary<Guid, Channel<string>>());
        var id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
        subs[id] = channel;
        reader = channel.Reader;
        return id;
    }

    public void Unsubscribe(string topic, Guid id)
    {
        if (_topics.TryGetValue(topic, out var subs) && subs.TryRemove(id, out var channel))
            channel.Writer.TryComplete();
    }

    public void Broadcast(string topic, string envelopeJson)
    {
        if (!_topics.TryGetValue(topic, out var subs)) return;
        foreach (var channel in subs.Values)
            channel.Writer.TryWrite(envelopeJson);
    }
}
