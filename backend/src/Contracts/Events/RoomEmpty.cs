using RedisBus;

namespace Contracts.Events;

public sealed record RoomEmpty(string RoomId, DateTimeOffset At) : IEvent;
