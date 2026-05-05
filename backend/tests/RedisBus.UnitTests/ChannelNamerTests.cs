using FluentAssertions;
using RedisBus.Internal;
using Xunit;

namespace RedisBus.UnitTests;

public class ChannelNamerTests
{
    private readonly ChannelNamer _namer = new();

    [Fact]
    public void Telemetry_uses_tel_prefix()
    {
        _namer.ForMessage(typeof(SampleMessages.TempReading))
            .Should().Be("tel.SampleMessages.TempReading");
    }

    [Fact]
    public void Event_uses_evt_prefix()
    {
        _namer.ForMessage(typeof(SampleMessages.RoomEmpty))
            .Should().Be("evt.SampleMessages.RoomEmpty");
    }

    [Fact]
    public void Query_uses_qry_prefix()
    {
        _namer.ForMessage(typeof(SampleMessages.GetCount))
            .Should().Be("qry.SampleMessages.GetCount");
    }

    [Fact]
    public void Request_uses_req_prefix()
    {
        _namer.ForMessage(typeof(SampleMessages.SetValue))
            .Should().Be("req.SampleMessages.SetValue");
    }

    [Fact]
    public void Command_uses_cmd_prefix()
    {
        _namer.ForMessage(typeof(SampleMessages.StartSimulation))
            .Should().Be("cmd.SampleMessages.StartSimulation");
    }

    [Fact]
    public void Reply_channel_includes_service_and_instance()
    {
        _namer.ForReply("api-gateway", "abc123")
            .Should().Be("rep.api-gateway.abc123");
    }

    [Fact]
    public void Type_without_marker_throws()
    {
        var act = () => _namer.ForMessage(typeof(SampleMessages.NotAMessage));
        act.Should().Throw<InvalidOperationException>();
    }
}
