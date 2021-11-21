using Microsoft.EntityFrameworkCore;

namespace aTES.Analytics.DataAccess
{
    public class AnalyticsDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ates_analytics;Username=postgres;Password=password");

        public DbSet<DbUser> Users { get; set; }

        public DbSet<DbTask> Tasks { get; set; }
    }
}
