using aTES.Billing.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class TasksService
    {
        private readonly Random _random;
        private readonly BillingDbContext _dbContext;
        private readonly MessageBus _messageBus;

        public TasksService(BillingDbContext dbContext, Random random, MessageBus messageBus)
        {
            _dbContext = dbContext;
            _random = random;
            _messageBus = messageBus;
        }

        public async Task StoreAndEstimate(string id, string jiraId, string description)
        {
            var task = new DbTask
            {
                Id = Guid.NewGuid(),
                JiraId = jiraId,
                Description = description,
                PublicId = id,
                BirdInCageCost = GenerateBirdInCageCost(),
                MilletInABowlCost = GenerateMilletInABowlCost(),
            };
            _dbContext.Add(task);
            await _dbContext.SaveChangesAsync();
            await _messageBus.SendTaskEstimatedEvent(task.PublicId, task.BirdInCageCost, task.MilletInABowlCost);
        }

        public async Task<ITask> GetById(string id)
        {
            return await _dbContext.Tasks.Include(x => x.AssignedUser).AsNoTracking().FirstAsync(x => x.PublicId == id);
        }

        private int GenerateBirdInCageCost()
        {
            //rand(-10..-20)$
            return -20 + _random.Next(10);
        }

        private int GenerateMilletInABowlCost()
        {
            //rand(20..40)$
            return 20 + _random.Next(20);
        }

        public async Task UpdateAssignee(string taskId, string assigneeId)
        {
            var user = await _dbContext.Users.FirstAsync(x => x.PublicId == assigneeId);
            var task = await _dbContext.Tasks.FirstAsync(x => x.PublicId == taskId);
            task.AssignedUser = user;
            await _dbContext.SaveChangesAsync();
        }
    }
}
