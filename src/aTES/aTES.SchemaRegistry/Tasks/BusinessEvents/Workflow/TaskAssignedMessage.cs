namespace aTES.SchemaRegistry.Tasks
{
    public class TaskAssignedMessage : IMessage
    {
        public string Id { get; set; }
        public string AssigneeId { get; set; }
    }
}
