using BuildingService.Domain;
using Contracts.Events;
using Contracts.Telemetry;
using RedisBus;

namespace BuildingService.Handlers;

public sealed class TelemetrySnapshotHandler :
    IHandle<TemperatureReading>,
    IHandle<OccupancyReading>
{
    private readonly Building _building;
    private readonly IMessageBus _bus;

    public TelemetrySnapshotHandler(Building building, IMessageBus bus)
    {
        _building = building;
        _bus = bus;
    }

    public Task HandleAsync(TemperatureReading message, MessageContext context, CancellationToken cancellationToken)
    {
        _building.Get(message.RoomId)?.SetTemperature(message.Celsius, message.At);
        return Task.CompletedTask;
    }

    public async Task HandleAsync(OccupancyReading message, MessageContext context, CancellationToken cancellationToken)
    {
        var room = _building.Get(message.RoomId);
        if (room is null) return;

        var changed = room.SetOccupancy(message.Occupied, message.At);
        if (!changed) return;

        await _bus.PublishAsync(
            new RoomOccupancyChanged(message.RoomId, message.Occupied, message.At),
            cancellationToken);
    }
}
