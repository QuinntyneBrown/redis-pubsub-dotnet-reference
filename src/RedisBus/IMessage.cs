namespace RedisBus;

public interface IMessage { }

public interface ITelemetry : IMessage { }

public interface IEvent : IMessage { }

public interface IQuery<TResponse> : IMessage { }

public interface IRequest<TResponse> : IMessage { }
