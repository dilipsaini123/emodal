using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ContainerLockAPI.Services;
using ContainerLockAPI.Models;

namespace ContainerLockAPI.Controllers
{
    [Route("api/container-lock")]
    [ApiController]
    public class ContainerLockController : ControllerBase
    {
        private readonly IContainerLockService _containerLockService;
        

        public ContainerLockController(IContainerLockService containerLockService)
        {
            _containerLockService = containerLockService;
        }

        [HttpPost("lock")]
        public async Task<IActionResult> LockContainer([FromBody] ContainerLock request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _containerLockService.LockContainerAsync(request.Username, request.ContainerId);

            if (result == "Locked")
                return Ok(new { message = "Container locked successfully." });
            else if (result == "AlreadyLocked")
                return Conflict(new { error = "Container is already locked by another user." });

            return Conflict(new { error = "Some container fees have already been paid." });
        }

        [HttpDelete("unlock")]
        public async Task<IActionResult> UnlockContainer([FromQuery] string containerId)
        {
            var success = await _containerLockService.ReleaseContainerLockAsync(containerId);
            if (success)
                return Ok(new { message = " Container lock released successfully." });

            return NotFound(new { error = " Container lock not found." });
        }

        [HttpGet("isLocked")]
        public async Task<IActionResult> IsContainerLocked([FromQuery] string containerId)
        {
            var isLocked = await _containerLockService.IsContainerLockedAsync(containerId);
            return Ok(new { containerId, isLocked });
        }
    }
}
