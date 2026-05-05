using ControlService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "control-service";
});
builder.Services.AddRedisBusHandlers(typeof(SetThermostatResponder).Assembly);

await builder.Build().RunAsync();
