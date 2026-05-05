using System.Text.Json;
using RedisBus;

namespace BffService.Endpoints;

public static class MessageEndpoints
{
    public sealed record SendBody(string Type, JsonElement Payload);

    public static void MapMessageEndpoints(this WebApplication app)
    {
        app.MapPost("/api/commands", Dispatch);
        app.MapPost("/api/queries", Dispatch);
        app.MapPost("/api/requests", Dispatch);
    }

    private static async Task<IResult> Dispatch(
        SendBody body, MessageBusBridge bridge, CancellationToken ct)
    {
        try
        {
            var result = await bridge.DispatchAsync(body.Type, body.Payload, ct);
            return Results.Ok(result);
        }
        catch (TypeNotFoundException ex)
        {
            return Results.NotFound(new { type = ex.TypeName });
        }
        catch (MessageValidationException ex)
        {
            return Results.BadRequest(new { errors = ex.Errors });
        }
        catch (TimeoutException)
        {
            return Results.StatusCode(StatusCodes.Status504GatewayTimeout);
        }
    }
}
