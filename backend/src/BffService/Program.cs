using BffService;
using BffService.Endpoints;
using BffService.Hubs;
using BffService.Subscriptions;
using Contracts;
using RedisBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisBus(o =>
{
    o.ConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    o.ServiceName = "bff";
});

builder.Services.AddSingleton(new MessageTypeRegistry(typeof(ContractsAssemblyMarker).Assembly));
builder.Services.AddSingleton<MessageBusBridge>();
builder.Services.AddSingleton<SubscriptionRegistry>();
builder.Services.AddHostedService<BusListener>();

builder.Services.AddSignalR();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true)));

var app = builder.Build();

app.UseCors();
app.MapMessageEndpoints();
app.MapHub<BffHub>("/hub");

app.Run();
