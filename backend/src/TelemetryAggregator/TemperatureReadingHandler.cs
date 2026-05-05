using Contracts.Events;
using Contracts.Telemetry;
using RedisBus;

namespace TelemetryAggregator;

public sealed class TemperatureReadingHandler : IHandle<TemperatureReading>
{
    private readonly RollingAggregates _aggregates;
    private readonly IMessageBus _bus;

    public TemperatureReadingHandler(RollingAggregates aggregates, IMessageBus bus)
    {
        _aggregates = aggregates;
        _bus = bus;
    }

    public async Task HandleAsync(TemperatureReading message, MessageContext context, CancellationToken cancellationToken)
    {
        var result = _aggregates.AddTemperature(message.RoomId, message.Celsius, message.At);
        if (!result.ExceededThreshold) return;

        await _bus.PublishAsync(
            new RoomTemperatureExceededThreshold(
                message.RoomId,
                Math.Round(result.Average, 2),
                RollingAggregates.TemperatureThresholdC,
                message.At),
            cancellationToken);
    }
}
