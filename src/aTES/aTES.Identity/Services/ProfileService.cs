using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aTES.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UsersStore _userStore;

        public ProfileService(UsersStore userStore)
        {
            _userStore = userStore;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userName = GetUserNameFromClaims(context.Subject);
            var user = await _userStore.GetByUsername(userName);

            var claims = new[]
            {
                new Claim("Role", user.Role.ToString()),
            };
            //NOTE: will be addded to JWT .Token.access_token
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userName = GetUserNameFromClaims(context.Subject);
            var user = await _userStore.FindByUsername(userName);
            context.IsActive = (user != null) && user.IsActive;
        }

        private string GetUserNameFromClaims(ClaimsPrincipal claims)
        {
            return claims.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
        }
    }
}
