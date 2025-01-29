using System.Threading.Tasks;

namespace ContainerLockAPI.Services
{
    public interface IContainerLockService
    {
        Task<string> LockContainerAsync(string username, string containerId);
        Task<bool> ReleaseContainerLockAsync(string containerId);
        Task<bool> IsContainerLockedAsync(string containerId);
    }
}
