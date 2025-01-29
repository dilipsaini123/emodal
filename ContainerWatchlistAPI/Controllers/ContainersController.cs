using ContainerWatchlistAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace ContainerWatchlistAPI.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController:ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public ContainersController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        // Get all containers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContainerData>>> GetContainers()
        {
            var containers = await _cosmosDbService.GetContainersAsync();
            return Ok(containers);
        }

        // Get container by Id

        [HttpGet("{containerId}")]
        public async Task<ActionResult<ContainerData>> GetContainerById(string containerId)
        {
            var container = await _cosmosDbService.GetContainerByIdAsync(containerId);
            if (container == null)
                return NotFound();  // Return 404 if the container was not found

            return Ok(container);
        }
    }
}
