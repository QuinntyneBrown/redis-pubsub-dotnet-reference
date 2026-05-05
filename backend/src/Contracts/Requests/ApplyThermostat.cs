using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace Contracts.Requests;

public sealed record ApplyThermostat(
    [property: Required] string DeviceId,
    [property: Required] string RoomId,
    [property: Range(10, 30)] double TargetC) : IRequest<ApplyThermostatAck>;

public sealed record ApplyThermostatAck(string DeviceId, double EffectiveC);
