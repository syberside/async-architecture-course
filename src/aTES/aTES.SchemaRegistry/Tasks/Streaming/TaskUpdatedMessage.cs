namespace aTES.SchemaRegistry.Tasks
{
    public class TaskUpdatedMessage : IMessage
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string AssigneeId { get; set; }

        public TaskStatus Status { get; set; }
    }
}
