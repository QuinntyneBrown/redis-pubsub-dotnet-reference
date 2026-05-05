using Contracts.Requests;
using RedisBus;

namespace ControlService;

public sealed class SetThermostatResponder : IRespond<SetThermostat, SetThermostatAck>
{
    private const double SafeMin = 15.0;
    private const double SafeMax = 28.0;

    private readonly IMessageBus _bus;

    public SetThermostatResponder(IMessageBus bus) => _bus = bus;

    public async Task<SetThermostatAck> RespondAsync(SetThermostat request, MessageContext context, CancellationToken cancellationToken)
    {
        if (request.TargetC < SafeMin || request.TargetC > SafeMax)
            throw new InvalidOperationException(
                $"Target {request.TargetC}°C is outside safe range [{SafeMin}, {SafeMax}].");

        var deviceId = $"thermo-{request.RoomId}";
        var apply = new ApplyThermostat(deviceId, request.RoomId, request.TargetC);
        var ack = await _bus.SendAsync(apply, null, cancellationToken);
        return new SetThermostatAck(request.RoomId, ack.DeviceId, ack.EffectiveC);
    }
}
