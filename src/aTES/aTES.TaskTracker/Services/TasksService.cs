using aTES.TaskTracker.DataLayer;
using aTES.TaskTracker.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace aTES.TaskTracker.Services
{
    public class TasksService
    {
        private readonly TasksDbContext _dbContext;
        private readonly MessageBus _messageBus;

        public TasksService(TasksDbContext dbContext, MessageBus messageBus)
        {
            _dbContext = dbContext;
            _messageBus = messageBus;
        }

        public bool IsValidTaskName(string name) => !name.Contains('[') && !name.Contains(']');

        public async Task Create(string description)
        {
            if (!IsValidTaskName(description))
            {
                throw new ArgumentException();
            }
            var taskId = Guid.NewGuid();
            var tasksDistribution = await DistibuteTasks(new[] { taskId });
            var assignee = tasksDistribution[taskId];
            var newTask = new DbTask
            {
                Id = taskId,
                AssignedUser = assignee,
                Description = description,
                IsCompeleted = false,
            };
            _dbContext.Add(newTask);
            await _dbContext.SaveChangesAsync();
            await _messageBus.SendTaskCreatedEvent(newTask);
            await _messageBus.SendTaskAssignedEvent(newTask);
            await _messageBus.SendTaskUpdatedStreamEvent(newTask);
        }

        public async Task<ITask[]> ListAll(string userPublicId)
        {
            //NOTE: case study only
            IQueryable<DbTask> query = _dbContext.Tasks;
            if (userPublicId != null)
            {
                query = query.Where(x => x.AssignedUser.PublicId == userPublicId);
            }
            return await query.Include(x => x.AssignedUser).AsNoTracking().ToArrayAsync();
        }

        public async Task DistributeOpenTasks()
        {
            var openTasks = await _dbContext.Tasks.Where(x => x.IsCompeleted == false).ToArrayAsync();
            var taskIds = openTasks.Select(x => x.Id).ToArray();
            var distribution = await DistibuteTasks(taskIds);
            foreach (var task in openTasks)
            {
                task.AssignedUser = distribution[task.Id];
            }
            await _dbContext.SaveChangesAsync();
            foreach (var task in openTasks)
            {
                await _messageBus.SendTaskAssignedEvent(task);
                await _messageBus.SendTaskUpdatedStreamEvent(task);
            }
        }

        public async Task<Roles> GetRole(string publicUserId)
        {
            var user = await _dbContext.Users.Where(x => x.PublicId == publicUserId).FirstAsync();
            return user.Role;
        }

        public async Task<bool> CompleteTask(Guid taskId, string userPublicId)
        {
            var task = await _dbContext.Tasks.Include(x => x.AssignedUser).FirstAsync(x => x.Id == taskId);
            if (task.AssignedUser.PublicId != userPublicId)
            {
                return false;
            }
            task.IsCompeleted = true;
            await _dbContext.SaveChangesAsync();
            await _messageBus.SendTaskCompletedEvent(task);
            await _messageBus.SendTaskUpdatedStreamEvent(task);
            return true;
        }

        [Pure]
        private async Task<IReadOnlyDictionary<Guid, DbUser>> DistibuteTasks(Guid[] taskIds)
        {
            var availableUsers = await _dbContext.Users
                .Where(x => x.Role != Roles.Admin && x.Role != Roles.Manager)
                .ToArrayAsync();
            var random = new Random(DateTime.Now.Millisecond);
            return taskIds.ToDictionary(x => x, x => availableUsers[random.Next(availableUsers.Length)]);
        }
    }
}
