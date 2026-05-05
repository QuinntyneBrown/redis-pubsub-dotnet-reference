namespace RedisBus.Internal;

internal sealed class ChannelNamer
{
    public string ForMessage(Type messageType)
    {
        var prefix = ResolvePrefix(messageType);
        return $"{prefix}.{ShortName(messageType)}";
    }

    public string ForReply(string serviceName, string instanceId)
        => $"rep.{serviceName}.{instanceId}";

    private static string ResolvePrefix(Type messageType)
    {
        if (typeof(ITelemetry).IsAssignableFrom(messageType)) return "tel";
        if (typeof(IEvent).IsAssignableFrom(messageType)) return "evt";

        foreach (var iface in messageType.GetInterfaces())
        {
            if (!iface.IsGenericType) continue;
            var def = iface.GetGenericTypeDefinition();
            if (def == typeof(IQuery<>)) return "qry";
            if (def == typeof(IRequest<>)) return "req";
            if (def == typeof(ICommand<>)) return "cmd";
        }

        throw new InvalidOperationException(
            $"{messageType.FullName} must implement ITelemetry, IEvent, IQuery<T>, IRequest<T>, or ICommand<T>.");
    }

    private static string ShortName(Type t)
    {
        var ns = t.Namespace ?? string.Empty;
        var lastSegmentStart = ns.LastIndexOf('.') + 1;
        var lastSegment = ns.Length == 0 ? string.Empty : ns[lastSegmentStart..];
        return lastSegment.Length == 0 ? t.Name : $"{lastSegment}.{t.Name}";
    }
}
