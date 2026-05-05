using ActuatorService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "actuator-service";
});
builder.Services.AddRedisBusHandlers(typeof(ApplyThermostatResponder).Assembly);
builder.Services.AddSingleton<ActuatorState>();

await builder.Build().RunAsync();
