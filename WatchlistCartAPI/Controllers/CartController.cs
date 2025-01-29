using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchlistCartAPI.Data;
using WatchlistCartAPI.Models;

namespace WatchlistCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCart(string username)
        {
            return await _context.Cart.Where(c => c.Username == username).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart(CartItem item)
        {
            _context.Cart.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCart), new { username = item.Username }, item);
        }

        [HttpDelete("{username}/{containerNumber}")]
        public async Task<IActionResult> RemoveFromCart(string username, string containerNumber)
        {
            var item = await _context.Cart
                .FirstOrDefaultAsync(c => c.Username == username && c.ContainerNumber == containerNumber);

            if (item == null)
            {
                return NotFound(new { Message = "Cart item not found" });
            }

            _context.Cart.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Container {containerNumber} removed from {username}'s cart" });
        }

    }
}
