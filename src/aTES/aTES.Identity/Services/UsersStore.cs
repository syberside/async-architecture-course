using aTES.Identity.DataLayer;
using aTES.Identity.Domain;
using IdentityModel;
using Microsoft.EntityFrameworkCore;
using System;
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

        // NOTE: Learning sample only
        public async Task<IUser[]> ListAll()
        {
            return await _dbContext.Users.AsNoTracking().ToArrayAsync();

        }

        public async Task Register(string login, string password, Roles role)
        {
            _dbContext.Users.Add(new DbUser
            {
                Id = Guid.NewGuid(),
                PasswordHash = password.ToSha256(),
                Role = role,
                Username = login,
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
