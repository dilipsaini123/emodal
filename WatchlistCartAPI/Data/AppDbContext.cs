using Microsoft.EntityFrameworkCore;
using WatchlistCartAPI.Models;

namespace WatchlistCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WatchlistItem> Watchlist { get; set; }
        public DbSet<CartItem> Cart { get; set; }
    }
}
