using System;

namespace aTES.SchemaRegistry.Tasks
{
    public class TaskUpdatedMessage_V1 : IVersionedMessage<TaskUpdatedMessage_V1.Data>
    {
        public const string EVENT_TYPE = "TaskUpdatedMessage";

        public Guid EventId { get; set; }

        public string EventType => EVENT_TYPE;

        public string EventVersion => "1";

        public DateTime EventCreatedAt { get; set; }

        public string EventProducer { get; set; }

        public Data Payload { get; set; }

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
