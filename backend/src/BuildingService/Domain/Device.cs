namespace BuildingService.Domain;

public enum DeviceKind { Sensor, Actuator }

public sealed record Device(string DeviceId, DeviceKind Kind, string Subkind, string RoomId);
