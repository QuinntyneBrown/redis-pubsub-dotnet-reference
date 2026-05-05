using System.Reflection;
using RedisBus;

namespace BffService;

public sealed class MessageTypeRegistry
{
    private readonly Dictionary<string, Type> _byName;

    public MessageTypeRegistry(Assembly contracts)
    {
        _byName = contracts.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IMessage).IsAssignableFrom(t))
            .ToDictionary(t => t.FullName!, t => t);
    }

    public Type? Resolve(string typeName)
        => _byName.TryGetValue(typeName, out var t) ? t : null;
}
