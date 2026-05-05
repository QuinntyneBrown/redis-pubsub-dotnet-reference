using BffService.Subscriptions;
using FluentAssertions;
using Xunit;

namespace BffService.UnitTests;

public class SubscriptionRegistryTests
{
    [Fact]
    public void Add_then_ConnectionsFor_returns_the_connection()
    {
        var registry = new SubscriptionRegistry();

        registry.Add("conn-1", "tel.Contracts.Telemetry.TemperatureReading");

        registry.ConnectionsFor("tel.Contracts.Telemetry.TemperatureReading")
            .Should().ContainSingle().Which.Should().Be("conn-1");
    }

    [Fact]
    public void Remove_drops_only_the_named_channel()
    {
        var registry = new SubscriptionRegistry();
        registry.Add("conn-1", "tel.A");
        registry.Add("conn-1", "tel.B");

        registry.Remove("conn-1", "tel.A");

        registry.ConnectionsFor("tel.A").Should().BeEmpty();
        registry.ConnectionsFor("tel.B").Should().ContainSingle().Which.Should().Be("conn-1");
    }

    [Fact]
    public void Clear_drops_all_channels_for_a_connection()
    {
        var registry = new SubscriptionRegistry();
        registry.Add("conn-1", "tel.A");
        registry.Add("conn-1", "tel.B");
        registry.Add("conn-2", "tel.A");

        registry.Clear("conn-1");

        registry.ConnectionsFor("tel.A").Should().ContainSingle().Which.Should().Be("conn-2");
        registry.ConnectionsFor("tel.B").Should().BeEmpty();
    }

    [Fact]
    public void ConnectionsFor_unknown_channel_is_empty()
    {
        var registry = new SubscriptionRegistry();
        registry.ConnectionsFor("nope").Should().BeEmpty();
    }
}
