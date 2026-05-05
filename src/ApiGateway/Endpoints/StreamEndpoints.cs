using ApiGateway.Streaming;

namespace ApiGateway.Endpoints;

public static class StreamEndpoints
{
    public static void MapStreamEndpoints(this WebApplication app)
    {
        app.MapGet("/stream/telemetry", (HttpContext ctx, SseBroker broker, CancellationToken ct)
            => Stream(ctx, broker, SseBroker.Telemetry, ct));

        app.MapGet("/stream/events", (HttpContext ctx, SseBroker broker, CancellationToken ct)
            => Stream(ctx, broker, SseBroker.Events, ct));
    }

    private static async Task Stream(HttpContext ctx, SseBroker broker, string topic, CancellationToken ct)
    {
        ctx.Response.ContentType = "text/event-stream";
        ctx.Response.Headers.CacheControl = "no-cache";

        var id = broker.Subscribe(topic, out var reader);
        try
        {
            await foreach (var json in reader.ReadAllAsync(ct))
            {
                await ctx.Response.WriteAsync($"data: {json}\n\n", ct);
                await ctx.Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { /* client disconnected */ }
        finally
        {
            broker.Unsubscribe(topic, id);
        }
    }
}
