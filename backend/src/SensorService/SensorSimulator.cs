using Contracts.Telemetry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedisBus;

namespace SensorService;

public sealed class SensorSimulator : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly SimulationOptions _options;
    private readonly ILogger<SensorSimulator> _logger;
    private readonly Random _random = new();

    public SensorSimulator(IMessageBus bus, IOptions<SimulationOptions> options, ILogger<SensorSimulator> logger)
    {
        _bus = bus;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SensorSimulator publishing for rooms {Rooms} every {Cadence}.",
            string.Join(",", _options.Rooms), _options.Cadence);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var room in _options.Rooms)
            {
                await PublishForRoom(room, stoppingToken);
            }
            await Task.Delay(_options.Cadence, stoppingToken);
        }
    }

    private async Task PublishForRoom(string room, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var temp = 19 + _random.NextDouble() * 6;            // 19–25
        var humidity = 35 + _random.NextDouble() * 25;       // 35–60
        var occupied = _random.NextDouble() > 0.4;

        await _bus.PublishAsync(new TemperatureReading($"temp-{room}", room, Math.Round(temp, 1), now), ct);
        await _bus.PublishAsync(new HumidityReading($"hum-{room}", room, Math.Round(humidity, 1), now), ct);
        await _bus.PublishAsync(new OccupancyReading($"occ-{room}", room, occupied, now), ct);
    }
}
