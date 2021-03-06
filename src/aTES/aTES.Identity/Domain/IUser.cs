namespace aTES.Identity.Domain
{
    public interface IUser
    {
        public string PublicId { get; }

        public string Username { get; }

        public string PasswordHash { get; }

        public bool IsActive { get; }

        public Roles Role { get; }
    }
}
