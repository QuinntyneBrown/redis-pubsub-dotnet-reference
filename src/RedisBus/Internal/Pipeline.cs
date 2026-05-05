using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace RedisBus.Internal;

internal sealed class Pipeline
{
    private readonly ILogger<Pipeline> _logger;

    public Pipeline(ILogger<Pipeline> logger) => _logger = logger;

    public Task RunSendAsync(object message, string channel, Guid messageId, Guid correlationId, Func<Task> publish)
    {
        _logger.LogInformation(
            "Outbound {Type} -> {Channel} (messageId {MessageId}, correlationId {CorrelationId})",
            message.GetType().Name, channel, messageId, correlationId);

        var errors = Validate(message);
        if (errors.Count > 0) throw new MessageValidationException(message.GetType(), errors);

        return publish();
    }

    public async Task RunReceiveOneWayAsync(object message, string channel, MessageContext context, Func<Task> handle)
    {
        LogInbound(message, channel, context);
        if (!TryValidate(message, channel, context)) return;
        try
        {
            await handle();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Handler for {Type} on {Channel} threw (correlationId {CorrelationId}); dropped.",
                message.GetType().Name, channel, context.CorrelationId);
        }
    }

    public async Task<object?> RunReceiveResponderAsync(object message, string channel, MessageContext context, Func<Task<object?>> respond)
    {
        LogInbound(message, channel, context);
        if (!TryValidate(message, channel, context)) return null;
        try
        {
            return await respond();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Responder for {Type} on {Channel} threw (correlationId {CorrelationId}); no reply sent.",
                message.GetType().Name, channel, context.CorrelationId);
            return null;
        }
    }

    private void LogInbound(object message, string channel, MessageContext context)
    {
        var latencyMs = (DateTimeOffset.UtcNow - context.OccurredAt).TotalMilliseconds;
        _logger.LogInformation(
            "Inbound {Type} <- {Channel} (messageId {MessageId}, correlationId {CorrelationId}, latency {LatencyMs}ms)",
            message.GetType().Name, channel, context.MessageId, context.CorrelationId, latencyMs);
    }

    private bool TryValidate(object message, string channel, MessageContext context)
    {
        var errors = Validate(message);
        if (errors.Count == 0) return true;
        _logger.LogWarning(
            "{Type} on {Channel} failed validation (correlationId {CorrelationId}): {Errors}",
            message.GetType().Name, channel, context.CorrelationId, string.Join("; ", errors));
        return false;
    }

    private static IReadOnlyList<string> Validate(object message)
    {
        var ctx = new ValidationContext(message);
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(message, ctx, results, validateAllProperties: true))
            return Array.Empty<string>();
        return results.Select(r => r.ErrorMessage ?? "Invalid").ToArray();
    }
}
