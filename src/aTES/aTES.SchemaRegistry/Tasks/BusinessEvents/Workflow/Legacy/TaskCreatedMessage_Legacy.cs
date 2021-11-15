namespace aTES.SchemaRegistry.Tasks
{
    public class TaskCreatedMessage_Legacy : IMessage
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
}
