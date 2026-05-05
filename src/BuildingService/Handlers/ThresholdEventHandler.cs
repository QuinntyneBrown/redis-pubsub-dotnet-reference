using BuildingService.Domain;
using Contracts.Events;
using RedisBus;

namespace BuildingService.Handlers;

public sealed class ThresholdEventHandler : IHandle<RoomTemperatureExceededThreshold>
{
    private readonly Building _building;

    public ThresholdEventHandler(Building building) => _building = building;

    public Task HandleAsync(RoomTemperatureExceededThreshold message, MessageContext context, CancellationToken cancellationToken)
    {
        _building.Get(message.RoomId)?.SetThresholdExceeded(true);
        return Task.CompletedTask;
    }
}
