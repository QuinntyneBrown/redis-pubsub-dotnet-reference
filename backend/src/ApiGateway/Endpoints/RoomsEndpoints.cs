using Contracts.Queries;
using Contracts.Requests;
using RedisBus;

namespace ApiGateway.Endpoints;

public static class RoomsEndpoints
{
    public sealed record SetThermostatBody(double TargetC);

    public static void MapRoomsEndpoints(this WebApplication app)
    {
        app.MapGet("/rooms", async (IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.SendAsync(new ListRooms(), null, ct);
            return Results.Ok(result.Rooms);
        });

        app.MapGet("/rooms/{id}", async (string id, IMessageBus bus, CancellationToken ct) =>
        {
            try
            {
                var status = await bus.SendAsync(new GetRoomStatus(id), null, ct);
                return Results.Ok(status);
            }
            catch (TimeoutException)
            {
                return Results.NotFound(new { roomId = id });
            }
        });

        app.MapPost("/rooms/{id}/thermostat", async (string id, SetThermostatBody body, IMessageBus bus, CancellationToken ct) =>
        {
            try
            {
                var ack = await bus.SendAsync(new SetThermostat(id, body.TargetC), null, ct);
                return Results.Ok(ack);
            }
            catch (MessageValidationException ex)
            {
                return Results.BadRequest(new { errors = ex.Errors });
            }
            catch (TimeoutException)
            {
                return Results.StatusCode(StatusCodes.Status504GatewayTimeout);
            }
        });
    }
}
