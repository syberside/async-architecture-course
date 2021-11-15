using System;
using static aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1.BirdInACageMessage_v1;

namespace aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1
{
    public class BirdInACageMessage_v1 : IVersionedMessage<Data>
    {
        public Guid EventId { get; set; }

        public string EventType => "BirdInACage";

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public string Id { get; set; }

            public string AssigneeId { get; set; }
        }
    }
}
