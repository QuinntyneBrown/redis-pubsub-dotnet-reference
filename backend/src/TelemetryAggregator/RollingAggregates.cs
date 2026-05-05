using System.Collections.Concurrent;

namespace TelemetryAggregator;

public sealed class RollingAggregates
{
    public const double TemperatureThresholdC = 24.0;
    public static readonly TimeSpan WindowSize = TimeSpan.FromMinutes(5);

    private readonly ConcurrentDictionary<string, RoomWindow> _byRoom = new();

    public AverageResult AddTemperature(string roomId, double celsius, DateTimeOffset at)
        => _byRoom.GetOrAdd(roomId, _ => new RoomWindow()).AddTemperature(celsius, at);

    public OccupancyResult AddOccupancy(string roomId, bool occupied, DateTimeOffset at)
        => _byRoom.GetOrAdd(roomId, _ => new RoomWindow()).AddOccupancy(occupied, at);

    public sealed record AverageResult(double Average, bool ExceededThreshold);
    public sealed record OccupancyResult(bool Occupied, bool TransitionedToEmpty);

    private sealed class RoomWindow
    {
        private readonly Queue<(DateTimeOffset At, double C)> _temps = new();
        private readonly object _lock = new();
        private bool? _lastOccupancy;

        public AverageResult AddTemperature(double celsius, DateTimeOffset at)
        {
            lock (_lock)
            {
                _temps.Enqueue((at, celsius));
                var cutoff = at - WindowSize;
                while (_temps.Count > 0 && _temps.Peek().At < cutoff)
                    _temps.Dequeue();

                var average = _temps.Average(x => x.C);
                return new AverageResult(average, average > TemperatureThresholdC);
            }
        }

        public OccupancyResult AddOccupancy(bool occupied, DateTimeOffset at)
        {
            lock (_lock)
            {
                var transitioned = !occupied && _lastOccupancy == true;
                _lastOccupancy = occupied;
                return new OccupancyResult(occupied, transitioned);
            }
        }
    }
}
