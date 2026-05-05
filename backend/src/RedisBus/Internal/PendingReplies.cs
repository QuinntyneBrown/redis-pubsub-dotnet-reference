using System.Collections.Concurrent;
using System.Text.Json;

namespace RedisBus.Internal;

internal sealed class PendingReplies
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<JsonElement>> _waiters = new();

    public Task<JsonElement> Register(Guid correlationId, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<JsonElement>(TaskCreationOptions.RunContinuationsAsynchronously);
        _waiters[correlationId] = tcs;

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);
        cts.Token.Register(() =>
        {
            if (_waiters.TryRemove(correlationId, out var pending))
            {
                pending.TrySetException(new TimeoutException(
                    $"No reply received for correlation id {correlationId} within {timeout}."));
            }
            cts.Dispose();
        });

        return tcs.Task;
    }

    public bool TryComplete(Guid correlationId, JsonElement payload)
    {
        if (!_waiters.TryRemove(correlationId, out var tcs)) return false;
        tcs.TrySetResult(payload);
        return true;
    }
}
