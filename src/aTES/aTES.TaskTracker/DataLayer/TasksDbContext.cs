using Microsoft.EntityFrameworkCore;

namespace aTES.TaskTracker.DataLayer
{
    public class TasksDbContext : DbContext
    {
        //NOTE: Hardcoded for case simplicity only
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ates_tasks;Username=postgres;Password=password");

        public virtual DbSet<DbUser> Users { get; set; }
        public virtual DbSet<DbTask> Tasks { get; set; }
    }
}
