using System.Linq;
using System.Security.Claims;

namespace aTES.Identity.Services
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUserName(this ClaimsPrincipal claims)
        {
            return claims.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
        }
    }
}
