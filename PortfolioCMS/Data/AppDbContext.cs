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

        // DbSet for the Project entity
        public DbSet<Project> Projects => Set<Project>();
    }
}
