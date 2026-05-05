using BffService;
using Contracts;
using Contracts.Queries;
using Contracts.Requests;
using FluentAssertions;
using Xunit;

namespace BffService.UnitTests;

public class MessageTypeRegistryTests
{
    private readonly MessageTypeRegistry _registry =
        new(typeof(ContractsAssemblyMarker).Assembly);

    [Fact]
    public void Resolves_known_query_type()
    {
        _registry.Resolve("Contracts.Queries.ListRooms")
            .Should().Be(typeof(ListRooms));
    }

    [Fact]
    public void Resolves_known_request_type()
    {
        _registry.Resolve("Contracts.Requests.SetThermostat")
            .Should().Be(typeof(SetThermostat));
    }

    [Fact]
    public void Returns_null_for_unknown_type()
    {
        _registry.Resolve("Nope.Missing").Should().BeNull();
    }
}
