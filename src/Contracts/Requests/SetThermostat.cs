using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Requests;

public sealed record SetThermostat(
    [property: Required] string RoomId,
    [property: Range(10, 30)] double TargetC) : IRequest<SetThermostatAck>;

public sealed record SetThermostatAck(string RoomId, string DeviceId, double EffectiveC);
