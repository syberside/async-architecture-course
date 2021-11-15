namespace aTES.SchemaRegistry.Users
{
    public class UserUpdatedMessage : IMessage
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Roles Role { get; set; }
    }
}
