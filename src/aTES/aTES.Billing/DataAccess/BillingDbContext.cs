using Microsoft.EntityFrameworkCore;

namespace aTES.Billing.DataAccess
{
    public class BillingDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ates_biling;Username=postgres;Password=password");

        public virtual DbSet<DbUser> Users { get; set; }
        public virtual DbSet<DbTask> Tasks { get; set; }
        public virtual DbSet<DbTransactionLogRecord> Transactions { get; set; }
    }
}
