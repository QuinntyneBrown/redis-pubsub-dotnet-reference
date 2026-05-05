using System.ComponentModel.DataAnnotations;
using RedisBus;

namespace RedisBus.UnitTests.SampleMessages;

public sealed record TempReading(
    [property: Required] string SensorId,
    [property: Range(-50, 100)] double Celsius) : ITelemetry;

public sealed record RoomEmpty(string RoomId) : IEvent;

public sealed record GetCount(string Bucket) : IQuery<CountResult>;
public sealed record CountResult(string Bucket, int Count);

public sealed record SetValue(
    [property: Required] string Key,
    [property: Range(0, 100)] int Value) : IRequest<SetValueAck>;
public sealed record SetValueAck(string Key, int Value);

public sealed record NotAMessage(string Foo);
