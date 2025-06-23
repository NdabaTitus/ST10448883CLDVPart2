using Microsoft.EntityFrameworkCore;

namespace EventEaseSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<Venues> Venues { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Bookings> Bookings { get; set; }

        public DbSet<EventType> EventType { get; set; }
    }
}
