using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;
using SensorService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "sensor-service";
});

builder.Services.Configure<SimulationOptions>(builder.Configuration.GetSection("Simulation"));
builder.Services.AddHostedService<SensorSimulator>();

await builder.Build().RunAsync();
