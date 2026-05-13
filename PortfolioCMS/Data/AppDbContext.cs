using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Models;

namespace PortfolioCMS.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor to pass options to the base DbContext class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet for the section entities
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Hero> Hero => Set<Hero>();
    }
}
