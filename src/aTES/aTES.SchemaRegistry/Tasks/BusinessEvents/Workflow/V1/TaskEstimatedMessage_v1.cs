using System;
using static aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1.TaskEstimatedMessage_v1;

namespace aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1
{
    public class TaskEstimatedMessage_v1 : IVersionedMessage<Data>
    {
        public const string EVENT_TYPE = "TaskEstimated";

        public Guid EventId { get; set; }

        public string EventType => EVENT_TYPE;

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public string Id { get; set; }
            public int BirdInACageCost { get; set; }
            public int MilletInABowlCost { get; set; }
        }
    }
}
