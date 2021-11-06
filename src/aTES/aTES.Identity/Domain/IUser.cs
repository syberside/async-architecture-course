namespace aTES.Identity.Services
{
    public interface IUser
    {
        public string SubjectId { get; }

        public string Username { get; }

        public string PasswordHash { get; }

        public bool IsActive { get; }

        public Roles Role { get; }
    }
}
