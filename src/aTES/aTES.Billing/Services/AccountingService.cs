using aTES.Billing.Controllers;
using aTES.Billing.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class AccountingService
    {
        private readonly BillingDbContext _dbContext;
        private readonly MessageBus _messageBus;

        public AccountingService(BillingDbContext dbContext, MessageBus messageBus)
        {
            _dbContext = dbContext;
            _messageBus = messageBus;
        }

        public async Task LogTransaction(string assigneeId, long credit, long debit, string details)
        {
            var user = await _dbContext.Users.FirstAsync(x => x.PublicId == assigneeId);
            var logRecord = new DbTransactionLogRecord
            {
                CreatedAt = DateTime.Now,
                Credit = credit,
                Debit = debit,
                Details = details,
                Id = Guid.NewGuid(),
                Owner = user,
            };
            _dbContext.Transactions.Add(logRecord);
            user.Balance += credit - debit;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(long Balance, IAuditLogEntry[] Log)> GetProfileData(string userId)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.PublicId == userId);
            var logRecords = await _dbContext.Transactions.AsNoTracking()
                .Where(x => x.OwnerId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .ToArrayAsync();
            return (user.Balance, logRecords);
        }

        public async Task<long> GetTodayTotal()
        {
            var today = DateTime.Now;
            var begin = new DateTime(today.Year, today.Month, today.Day);
            var end = begin.AddDays(1).AddSeconds(-1);
            var result = await _dbContext.Transactions.AsNoTracking()
                .Where(x => begin <= x.CreatedAt && x.CreatedAt <= end)
                .SumAsync(x => x.Credit + x.Debit);
            return result * -1;
        }

        public async Task CloseDay()
        {
            await _messageBus.SendOperationDayClosedEvent();
        }
    }
}
