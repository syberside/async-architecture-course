using Microsoft.EntityFrameworkCore;

namespace aTES.Identity.DataLayer
{
    public class IdentityDbContext : DbContext
    {
        //NOTE: Hardcoded for case simplicity only
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ates_identity;Username=postgres;Password=password");

        public virtual DbSet<DbUser> Users { get; set; }
    }
}
