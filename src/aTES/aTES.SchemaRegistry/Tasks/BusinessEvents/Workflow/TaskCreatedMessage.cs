namespace aTES.SchemaRegistry.Tasks
{
    public class TaskCreatedMessage : IMessage
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
}
