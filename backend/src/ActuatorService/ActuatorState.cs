using System.Collections.Concurrent;

namespace ActuatorService;

public sealed class ActuatorState
{
    private readonly ConcurrentDictionary<string, double> _thermostatTargets = new();

    public double SetThermostat(string deviceId, double target)
    {
        _thermostatTargets[deviceId] = target;
        return target;
    }

    public double? GetThermostat(string deviceId)
        => _thermostatTargets.TryGetValue(deviceId, out var v) ? v : null;
}
