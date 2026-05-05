using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisBus.IntegrationTests.TestMessages;
using Xunit;

namespace RedisBus.IntegrationTests;

[Collection(RedisCollection.Name)]
public class QueryResponseTests
{
    private readonly RedisFixture _redis;

    public QueryResponseTests(RedisFixture redis) => _redis = redis;

    [Fact]
    public async Task Issuer_receives_correlated_response()
    {
        using var responder = await _redis.StartServiceAsync("responder-qry", typeof(SampleQueryResponder).Assembly);
        using var issuer = await _redis.StartServiceAsync("issuer-qry", typeof(SampleQueryResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var result = await bus.SendAsync(new SampleQuery("hello"));

        result.Key.Should().Be("hello");
        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task SendAsync_throws_TimeoutException_when_no_responder_replies()
    {
        using var issuer = await _redis.StartServiceAsync("issuer-timeout", typeof(SampleQueryResponder).Assembly);
        var bus = issuer.Services.GetRequiredService<IMessageBus>();

        var act = () => bus.SendAsync(new SampleQuery("x"), TimeSpan.FromMilliseconds(200));

        await act.Should().ThrowAsync<TimeoutException>();
    }
}
