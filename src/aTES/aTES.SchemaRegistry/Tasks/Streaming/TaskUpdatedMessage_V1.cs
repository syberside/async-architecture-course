using System;

namespace aTES.SchemaRegistry.Tasks
{
    public class TaskUpdatedMessage_V1 : IVersionedMessage<TaskUpdatedMessage_V1.Data>
    {
        public Guid EventId { get; set; }

        public string EventType => "TaskUpdatedMessage";

        public string EventVersion => "V1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public TaskUpdatedMessage_V1.Data Payload { get; set; }

        public class Data : IMessagePayload
        {
            public string Id { get; set; }

            public string Description { get; set; }

            public string JiraId { get; set; }

            public string AssigneeId { get; set; }

            public TaskStatus Status { get; set; }
        }
    }
}
