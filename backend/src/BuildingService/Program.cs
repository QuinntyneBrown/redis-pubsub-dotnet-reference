using BuildingService.Domain;
using BuildingService.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "building-service";
});
builder.Services.AddRedisBusHandlers(typeof(TelemetrySnapshotHandler).Assembly);
builder.Services.AddSingleton<Building>();

await builder.Build().RunAsync();
