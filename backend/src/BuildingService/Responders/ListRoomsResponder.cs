using BuildingService.Domain;
using Contracts.Queries;
using RedisBus;

namespace BuildingService.Responders;

public sealed class ListRoomsResponder : IRespond<ListRooms, ListRoomsResult>
{
    private readonly Building _building;

    public ListRoomsResponder(Building building) => _building = building;

    public Task<ListRoomsResult> RespondAsync(ListRooms query, MessageContext context, CancellationToken cancellationToken)
    {
        var summaries = _building.Rooms
            .Select(r => new RoomSummary(r.Id, r.Name, r.Occupied, r.LastTemperatureC, r.LastUpdated))
            .ToArray();
        return Task.FromResult(new ListRoomsResult(summaries));
    }
}
