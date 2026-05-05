using System.Text.Json;
using FluentAssertions;
using RedisBus.Internal;
using Xunit;

namespace RedisBus.UnitTests;

public class PendingRepliesTests
{
    private readonly PendingReplies _pending = new();

    [Fact]
    public async Task Completes_when_reply_arrives()
    {
        var correlationId = Guid.NewGuid();
        var task = _pending.Register(correlationId, TimeSpan.FromSeconds(2), CancellationToken.None);

        var payload = JsonSerializer.SerializeToElement(new { Value = 42 });
        var completed = _pending.TryComplete(correlationId, payload);

        completed.Should().BeTrue();
        var result = await task;
        result.GetProperty("Value").GetInt32().Should().Be(42);
    }

    [Fact]
    public async Task Throws_TimeoutException_when_no_reply_arrives()
    {
        var correlationId = Guid.NewGuid();
        var task = _pending.Register(correlationId, TimeSpan.FromMilliseconds(50), CancellationToken.None);

        var act = () => task;
        await act.Should().ThrowAsync<TimeoutException>();
    }

    [Fact]
    public void Late_reply_for_unknown_correlation_returns_false()
    {
        var payload = JsonSerializer.SerializeToElement(new { Value = 1 });
        _pending.TryComplete(Guid.NewGuid(), payload).Should().BeFalse();
    }
}
