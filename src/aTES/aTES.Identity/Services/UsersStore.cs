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
        private readonly MessageBus _messageBus;

        public UsersStore(IdentityDbContext dbContext, MessageBus messageBus)
        {
            _dbContext = dbContext;
            _messageBus = messageBus;
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
            var newUser = new DbUser
            {
                Id = Guid.NewGuid(),
                PasswordHash = password.ToSha256(),
                Role = role,
                Username = login,
            };
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            await _messageBus.SendUserUpdatedStreamEvent(newUser);
        }

        public async Task UpdateRole(Guid id, Roles role)
        {
            var user = await _dbContext.Users.FirstAsync(x => x.Id == id);
            user.Role = role;
            await _dbContext.SaveChangesAsync();
            await _messageBus.SendUserUpdatedStreamEvent(user);
        }
    }
}
