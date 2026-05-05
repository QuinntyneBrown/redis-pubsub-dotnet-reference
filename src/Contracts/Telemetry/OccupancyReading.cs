using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Telemetry;

public sealed record OccupancyReading(
    [property: Required] string SensorId,
    [property: Required] string RoomId,
    bool Occupied,
    DateTimeOffset At) : ITelemetry;
