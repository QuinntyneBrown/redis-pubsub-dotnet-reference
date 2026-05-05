using RedisBus;

namespace Contracts.Events;

public sealed record RoomOccupancyChanged(string RoomId, bool Occupied, DateTimeOffset At) : IEvent;
