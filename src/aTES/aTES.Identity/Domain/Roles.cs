namespace aTES.Identity.Domain
{
    public enum Roles
    {
        SuperUser = 0,
        Admin = 1,
        RegularPopug = 2,
        Manager = 3,
        Acounter = 4,
    }

    public static class RolesExtension
    {
        public static bool CanAdministerUsers(this Roles role)
        {
            return role == Roles.SuperUser || role == Roles.Admin;
        }
    }
}
