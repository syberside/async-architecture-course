using System;
using static aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1.MilletInABowlMessage_v1;

namespace aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1
{
    public class MilletInABowlMessage_v1 : IVersionedMessage<Data>
    {
        public Guid EventId { get; set; }

        public string EventType => "MilletInABowl_v1";

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public string Id { get; set; }
        }
    }
}
