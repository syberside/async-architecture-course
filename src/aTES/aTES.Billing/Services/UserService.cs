using aTES.Billing.DataAccess;
using aTES.Billing.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace aTES.Billing.Controllers
{
    public class UserService
    {
        private readonly BillingDbContext _dbContext;

        public UserService(BillingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Roles> GetRole(string publicId)
        {
            var user = await _dbContext.Users.FirstAsync(x => x.PublicId == publicId);
            return user.Role;
        }
    }
}
