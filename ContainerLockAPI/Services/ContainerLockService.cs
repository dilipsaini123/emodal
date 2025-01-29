using ContainerLockAPI.Data;
using ContainerLockAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerLockAPI.Services
{
    public class ContainerLockService : IContainerLockService
    {
        private readonly ApplicationDbContext _dbContext;

        public ContainerLockService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> LockContainerAsync(string username, string containerId)
        {
            var existingLock = await _dbContext.ContainerLock
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync(c => c.ContainerId == containerId);

            if (existingLock != null)
            {
                if (existingLock.LockTimestamp.AddMinutes(1) <= DateTime.UtcNow && existingLock.Status != "Paid")
                {
                    _dbContext.ContainerLock.Remove(existingLock);
                    await _dbContext.SaveChangesAsync();
                }
                else if(existingLock.Status == "Paid")
                {
                    return "Paid";
                }
                else{
                    return "AlreadyLocked";
                }
                
            }

            var newLock = new ContainerLock
            {
                ContainerId = containerId,
                Username = username,
                Status = "Locked"
            };

            _dbContext.ContainerLock.Add(newLock);
            await _dbContext.SaveChangesAsync();
            return "Locked";
        }



        public async Task<bool> ReleaseContainerLockAsync(string containerId)
        {
            var existingLock = await _dbContext.ContainerLock.FindAsync(containerId);
            if (existingLock == null)
                return false;

            // _dbContext.ContainerLock.Remove(existingLock);
            existingLock.Status = "Paid"; 
            _dbContext.ContainerLock.Update(existingLock);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsContainerLockedAsync(string containerId)
        {
            var existingLock = await _dbContext.ContainerLock.FindAsync(containerId);
            return existingLock != null && existingLock.LockTimestamp.AddMinutes(1) > DateTime.UtcNow;
        }
    }
}
