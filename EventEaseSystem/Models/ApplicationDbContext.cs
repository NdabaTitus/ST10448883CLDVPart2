using Microsoft.EntityFrameworkCore;

namespace EventEaseSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
    }
}
