using aTES.Identity.DataLayer;
using IdentityModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace aTES.Identity.Services
{
    public class UsersStore
    {
        private readonly IdentityDbContext _dbContext;

        public UsersStore(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            var user = await FindByUsername(username);
            if (user == null)
            {
                return false;
            }
            return user.PasswordHash == password.ToSha256();
        }

        public async Task<IUser> FindByUsername(string username)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IUser> GetByUsername(string username)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}
