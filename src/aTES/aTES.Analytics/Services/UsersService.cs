using aTES.Analytics.DataAccess;
using aTES.Analytics.Domain;
using aTES.Analytics.Models;
using Microsoft.EntityFrameworkCore;

namespace aTES.Analytics.Services
{
    public class UsersService
    {
        private readonly AnalyticsDbContext _dbContext;

        public UsersService(AnalyticsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Roles> GetRole(string publicId)
        {
            var user = await _dbContext.Users.FirstAsync(x => x.PublicId == publicId);
            return user.Role;
        }

        public async Task<int> CountDeptors()
        {
            return await _dbContext.Users.Where(x => x.Balance < 0).CountAsync();
        }

        public async Task<long> GetBalanceTotal()
        {
            return -1 * await _dbContext.Users.Select(x => x.Balance).SumAsync();
        }

        public async Task<long> GetMostExpensiveTaskCost(DashboardType type)
        {
            DateTime start;
            DateTime end;
            switch (type)
            {
                case DashboardType.MostExpensiveByDay:
                    start = DateTime.Today;
                    end = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    break;
                case DashboardType.MostExpensiveByWeek:
                    start = DateTime.Today;
                    while (start.DayOfWeek != DayOfWeek.Monday) start = start.AddDays(-1);
                    end = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    while (end.DayOfWeek != DayOfWeek.Sunday) end = end.AddDays(1);
                    break;
                case DashboardType.MostExpensiveByMonth:
                    var today = DateTime.Today;
                    start = new DateTime(today.Year, today.Month, 1);
                    end = start.AddMonths(1).AddMilliseconds(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));

            }
            return await _dbContext.Tasks.Where(x => start <= x.ClosedAt && x.ClosedAt <= end).Select(x => (long?)x.Cost).MaxAsync()??0;
        }
    }
}
