using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Telemetry;

public sealed record HumidityReading(
    [property: Required] string SensorId,
    [property: Required] string RoomId,
    [property: Range(0, 100)] double Percent,
    DateTimeOffset At) : ITelemetry;
