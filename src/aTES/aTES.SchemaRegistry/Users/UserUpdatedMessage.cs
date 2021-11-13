namespace aTES.SchemaRegistry.Users
{
    public class UserUpdatedMessage
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Roles Role { get; set; }
    }
}
