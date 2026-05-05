using Contracts.Events;
using Contracts.Telemetry;
using RedisBus;

namespace TelemetryAggregator;

public sealed class OccupancyReadingHandler : IHandle<OccupancyReading>
{
    private readonly RollingAggregates _aggregates;
    private readonly IMessageBus _bus;

    public OccupancyReadingHandler(RollingAggregates aggregates, IMessageBus bus)
    {
        _aggregates = aggregates;
        _bus = bus;
    }

    public async Task HandleAsync(OccupancyReading message, MessageContext context, CancellationToken cancellationToken)
    {
        var result = _aggregates.AddOccupancy(message.RoomId, message.Occupied, message.At);
        if (!result.TransitionedToEmpty) return;

        await _bus.PublishAsync(new RoomEmpty(message.RoomId, message.At), cancellationToken);
    }
}
