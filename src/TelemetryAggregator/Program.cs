using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;
using TelemetryAggregator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "telemetry-aggregator";
});
builder.Services.AddRedisBusHandlers(typeof(TemperatureReadingHandler).Assembly);
builder.Services.AddSingleton<RollingAggregates>();

await builder.Build().RunAsync();
