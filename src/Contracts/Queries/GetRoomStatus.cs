using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Queries;

public sealed record GetRoomStatus([property: Required] string RoomId) : IQuery<RoomStatus>;

public sealed record RoomStatus(
    string RoomId,
    string Name,
    double? TemperatureC,
    bool Occupied,
    DateTimeOffset? LastUpdated,
    IReadOnlyList<string> Devices);
