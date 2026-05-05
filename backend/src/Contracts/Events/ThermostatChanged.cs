using RedisBus;

namespace Contracts.Events;

public sealed record ThermostatChanged(
    string DeviceId,
    string RoomId,
    double EffectiveC,
    DateTimeOffset At) : IEvent;
