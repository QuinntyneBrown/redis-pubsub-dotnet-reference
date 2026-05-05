namespace BuildingService.Domain;

public sealed class Room
{
    private readonly object _lock = new();
    private readonly List<Device> _devices = new();

    public Room(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }
    public string Name { get; }
    public IReadOnlyList<Device> Devices => _devices;
    public double? LastTemperatureC { get; private set; }
    public bool Occupied { get; private set; }
    public DateTimeOffset? LastUpdated { get; private set; }
    public bool ThresholdExceeded { get; private set; }

    public void AddDevice(Device device) => _devices.Add(device);

    public void SetTemperature(double celsius, DateTimeOffset at)
    {
        lock (_lock)
        {
            LastTemperatureC = celsius;
            LastUpdated = at;
        }
    }

    public bool SetOccupancy(bool occupied, DateTimeOffset at)
    {
        lock (_lock)
        {
            var changed = Occupied != occupied;
            Occupied = occupied;
            LastUpdated = at;
            return changed;
        }
    }

    public void SetThresholdExceeded(bool exceeded)
    {
        lock (_lock) { ThresholdExceeded = exceeded; }
    }
}
