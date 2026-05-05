using BuildingService.Domain;
using Contracts.Queries;
using RedisBus;

namespace BuildingService.Responders;

public sealed class GetRoomStatusResponder : IRespond<GetRoomStatus, RoomStatus>
{
    private readonly Building _building;

    public GetRoomStatusResponder(Building building) => _building = building;

    public Task<RoomStatus> RespondAsync(GetRoomStatus query, MessageContext context, CancellationToken cancellationToken)
    {
        var room = _building.Get(query.RoomId)
            ?? throw new KeyNotFoundException($"Room '{query.RoomId}' not found.");

        var devices = room.Devices.Select(d => d.DeviceId).ToArray();
        return Task.FromResult(new RoomStatus(
            room.Id, room.Name, room.LastTemperatureC, room.Occupied, room.LastUpdated, devices));
    }
}
