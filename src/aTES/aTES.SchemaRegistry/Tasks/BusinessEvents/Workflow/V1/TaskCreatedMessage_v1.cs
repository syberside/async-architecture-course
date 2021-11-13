using System;
using static aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1.TaskCreatedMessage_v1;

namespace aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1
{
    public class TaskCreatedMessage_v1 : IVersionedMessage<Data>
    {
        public Guid EventId { get; set; }

        public string EventType => "TaskCreated";

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public string Id { get; set; }

            public string Description { get; set; }

            public string JiraId { get; set; }
        }
    }
}
