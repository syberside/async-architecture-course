using System;
using static aTES.SchemaRegistry.Billing.OperationDayClosedMessage_v1;

namespace aTES.SchemaRegistry.Billing
{
    public class OperationDayClosedMessage_v1 : IVersionedMessage<Data>
    {
        public const string EVENT_TYPE = "OperationDayClosed";

        public Guid EventId { get; set; }

        public string EventType => EVENT_TYPE;

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public DateTime Day { get; set; }
        }
    }
}
