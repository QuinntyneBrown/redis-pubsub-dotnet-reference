using Contracts.Events;
using Contracts.Requests;
using RedisBus;

namespace ActuatorService;

public sealed class ApplyThermostatResponder : IRespond<ApplyThermostat, ApplyThermostatAck>
{
    private readonly ActuatorState _state;
    private readonly IMessageBus _bus;

    public ApplyThermostatResponder(ActuatorState state, IMessageBus bus)
    {
        _state = state;
        _bus = bus;
    }

    public async Task<ApplyThermostatAck> RespondAsync(ApplyThermostat request, MessageContext context, CancellationToken cancellationToken)
    {
        var effective = _state.SetThermostat(request.DeviceId, request.TargetC);

        await _bus.PublishAsync(
            new ThermostatChanged(request.DeviceId, request.RoomId, effective, DateTimeOffset.UtcNow),
            cancellationToken);

        return new ApplyThermostatAck(request.DeviceId, effective);
    }
}
