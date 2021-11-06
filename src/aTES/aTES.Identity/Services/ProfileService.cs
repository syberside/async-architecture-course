using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aTES.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UsersStore _usersStore;

        public ProfileService(UsersStore usersStore)
        {
            _usersStore = usersStore;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userName = context.Subject.GetUserName();
            var user = await _usersStore.GetByUsername(userName);

            var claims = new[]
            {
                new Claim("Role", user.Role.ToString()),
            };
            //NOTE: will be addded to JWT .Token.access_token
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userName = context.Subject.GetUserName();
            var user = await _usersStore.FindByUsername(userName);
            context.IsActive = (user != null) && user.IsActive;
        }
    }
}
