using RedisBus;

namespace Contracts.Queries;

public sealed record ListRooms() : IQuery<ListRoomsResult>;

public sealed record ListRoomsResult(IReadOnlyList<RoomSummary> Rooms);

public sealed record RoomSummary(
    string RoomId,
    string Name,
    bool Occupied,
    double? LastTemperatureC,
    DateTimeOffset? LastUpdated);
