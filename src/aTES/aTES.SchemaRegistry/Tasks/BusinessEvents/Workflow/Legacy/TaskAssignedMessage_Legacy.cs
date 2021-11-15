namespace aTES.SchemaRegistry.Tasks
{
    public class TaskAssignedMessage_Legacy : IMessage
    {
        public string Id { get; set; }
        public string AssigneeId { get; set; }
    }
}
