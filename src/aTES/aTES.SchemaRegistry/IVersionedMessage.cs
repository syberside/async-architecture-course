using System;

namespace aTES.SchemaRegistry
{
    public interface IVersionedMessage<T> : IMessage
        where T : IMessagePayload
    {
        Guid EventId { get; }
        string EventType { get; }
        string EventVersion { get; }
        DateTime EventCreatedAt { get; }
        string EventProducer { get; }
        T Payload { get; }
    }
}
