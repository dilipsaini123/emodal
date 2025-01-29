using Microsoft.EntityFrameworkCore;
using ContainerLockAPI.Models;

namespace ContainerLockAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ContainerLock> ContainerLock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {  
            modelBuilder.Entity<ContainerLock>()
                .HasKey(c => c.ContainerId);

            modelBuilder.Entity<ContainerLock>()
                .Property(c => c.LockTimestamp)
                .HasDefaultValueSql("GETUTCDATE()"); 

            

               
        }

       
        public async Task CleanupExpiredLocksAsync()
        {
            var expiredLocks = ContainerLock.Where(c => c.LockTimestamp.AddMinutes(1) <= DateTime.UtcNow);
            ContainerLock.RemoveRange(expiredLocks);
            await SaveChangesAsync();
        }
        
    }
}
