using RedisBus;

namespace Contracts.Events;

public sealed record RoomTemperatureExceededThreshold(
    string RoomId,
    double AverageC,
    double ThresholdC,
    DateTimeOffset At) : IEvent;
