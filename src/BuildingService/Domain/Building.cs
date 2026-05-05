using System.Collections.Concurrent;

namespace BuildingService.Domain;

public sealed class Building
{
    private readonly ConcurrentDictionary<string, Room> _rooms = new();

    public Building()
    {
        Seed("R-101", "Lobby");
        Seed("R-102", "Lab");
        Seed("R-103", "Lounge");
    }

    public IEnumerable<Room> Rooms => _rooms.Values;

    public Room? Get(string roomId) => _rooms.GetValueOrDefault(roomId);

    private void Seed(string roomId, string name)
    {
        var room = new Room(roomId, name);
        room.AddDevice(new Device($"temp-{roomId}", DeviceKind.Sensor, "Temperature", roomId));
        room.AddDevice(new Device($"occ-{roomId}", DeviceKind.Sensor, "OccupancyDetector", roomId));
        room.AddDevice(new Device($"thermo-{roomId}", DeviceKind.Actuator, "Thermostat", roomId));
        _rooms[roomId] = room;
    }
}
