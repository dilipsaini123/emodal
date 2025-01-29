using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchlistCartAPI.Data;
using WatchlistCartAPI.Models;

namespace WatchlistCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WatchlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<WatchlistItem>>> GetWatchlist(string username)
        {
            return await _context.Watchlist.Where(w => w.Username == username).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<WatchlistItem>> AddToWatchlist(WatchlistItem item)
        {
            _context.Watchlist.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetWatchlist), new { username = item.Username }, item);
        }

        [HttpDelete("{username}/{containerNumber}")]
        public async Task<IActionResult> RemoveFromWatchlist(string username, string containerNumber)
        {
            var item = await _context.Watchlist
                .FirstOrDefaultAsync(w => w.Username == username && w.ContainerNumber == containerNumber);

            if (item == null)
            {
                return NotFound(new { Message = "Watchlist item not found" });
            }

            _context.Watchlist.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Container {containerNumber} removed from {username}'s watchlist" });
        }

    }
}
