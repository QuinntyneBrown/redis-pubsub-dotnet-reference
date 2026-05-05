using System.Text.Json;
using RedisBus;

namespace BffService;

public sealed class MessageBusBridge
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IMessageBus _bus;
    private readonly MessageTypeRegistry _types;

    public MessageBusBridge(IMessageBus bus, MessageTypeRegistry types)
    {
        _bus = bus;
        _types = types;
    }

    public async Task<object?> DispatchAsync(string typeName, JsonElement payload, CancellationToken ct)
    {
        var clrType = _types.Resolve(typeName)
            ?? throw new TypeNotFoundException(typeName);

        dynamic message = payload.Deserialize(clrType, JsonOptions)
            ?? throw new InvalidOperationException($"Payload for {typeName} deserialized to null.");

        // dynamic lets the runtime pick the IMessageBus.SendAsync overload that matches
        // the message's marker interface (IQuery / IRequest / ICommand).
        return await _bus.SendAsync(message, null, ct);
    }
}

public sealed class TypeNotFoundException(string typeName)
    : Exception($"Unknown message type '{typeName}'.")
{
    public string TypeName { get; } = typeName;
}
