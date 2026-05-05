using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using RedisBus;

namespace RedisBus.IntegrationTests.TestMessages;

public sealed record SampleTelemetry([property: Required] string SensorId, double Value) : ITelemetry;
public sealed record SampleEvent([property: Required] string Source, string Detail) : IEvent;
public sealed record SampleQuery([property: Required] string Key) : IQuery<SampleQueryResult>;
public sealed record SampleQueryResult(string Key, int Count);
public sealed record SampleRequest(
    [property: Required] string Key,
    [property: Range(0, 100)] int Value) : IRequest<SampleRequestAck>;
public sealed record SampleRequestAck(string Key, int Applied);

public sealed record SampleCommand(
    [property: Required] string Profile) : ICommand<SampleCommandAck>;
public sealed record SampleCommandAck(string SessionId, string Profile);

public static class Inbox
{
    public static ConcurrentBag<object> Received { get; } = new();
    public static void Clear() { while (Received.TryTake(out _)) { } }
}

public sealed class SampleTelemetryHandler : IHandle<SampleTelemetry>
{
    public Task HandleAsync(SampleTelemetry message, MessageContext context, CancellationToken cancellationToken)
    {
        Inbox.Received.Add(message);
        return Task.CompletedTask;
    }
}

public sealed class SampleEventHandler : IHandle<SampleEvent>
{
    public Task HandleAsync(SampleEvent message, MessageContext context, CancellationToken cancellationToken)
    {
        Inbox.Received.Add(message);
        return Task.CompletedTask;
    }
}

public sealed class SampleQueryResponder : IRespond<SampleQuery, SampleQueryResult>
{
    public Task<SampleQueryResult> RespondAsync(SampleQuery message, MessageContext context, CancellationToken cancellationToken)
        => Task.FromResult(new SampleQueryResult(message.Key, message.Key.Length));
}

public sealed class SampleRequestResponder : IRespond<SampleRequest, SampleRequestAck>
{
    public Task<SampleRequestAck> RespondAsync(SampleRequest message, MessageContext context, CancellationToken cancellationToken)
        => Task.FromResult(new SampleRequestAck(message.Key, message.Value));
}

public sealed class SampleCommandResponder : IRespond<SampleCommand, SampleCommandAck>
{
    public Task<SampleCommandAck> RespondAsync(SampleCommand message, MessageContext context, CancellationToken cancellationToken)
        => Task.FromResult(new SampleCommandAck("session-" + message.Profile, message.Profile));
}
