using ApiGateway.Endpoints;
using ApiGateway.Streaming;
using RedisBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "api-gateway";
});

builder.Services.AddSingleton<SseBroker>();
builder.Services.AddHostedService<TelemetryStreamHandler>();
builder.Services.AddHostedService<EventStreamHandler>();

var app = builder.Build();

app.MapRoomsEndpoints();
app.MapStreamEndpoints();

app.Run();
