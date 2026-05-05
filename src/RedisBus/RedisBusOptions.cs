using System.Reflection;

namespace RedisBus;

public sealed class RedisBusOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";

    public string ServiceName { get; set; } = "service";

    public string InstanceId { get; set; } = Guid.NewGuid().ToString("N")[..8];

    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public IList<Assembly> HandlerAssemblies { get; } = new List<Assembly>();
}
