using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class WebAPIDbContext : DbContext
    {
        public WebAPIDbContext(DbContextOptions<WebAPIDbContext> options) : base(options)
        {
            //
        }

        public DbSet<User> Users { get; set; }
    }
}
