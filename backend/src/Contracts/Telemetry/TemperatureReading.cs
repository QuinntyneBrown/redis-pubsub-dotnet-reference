using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Telemetry;

public sealed record TemperatureReading(
    [property: Required] string SensorId,
    [property: Required] string RoomId,
    [property: Range(-50, 100)] double Celsius,
    DateTimeOffset At) : ITelemetry;
