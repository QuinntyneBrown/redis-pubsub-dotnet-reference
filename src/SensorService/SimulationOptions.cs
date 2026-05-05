namespace SensorService;

public sealed class SimulationOptions
{
    public IList<string> Rooms { get; set; } = new List<string> { "R-101", "R-102", "R-103" };

    public TimeSpan Cadence { get; set; } = TimeSpan.FromSeconds(2);
}
