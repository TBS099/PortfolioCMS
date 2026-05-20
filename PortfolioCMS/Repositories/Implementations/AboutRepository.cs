using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;

namespace PortfolioCMS.Repositories.Implementations
{
    public class AboutRepository : IAboutRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public AboutRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get the about section
        public async Task<About?> GetAboutAsync()
        {
            return await _context.About.FirstOrDefaultAsync();
        }

        // Create the about section
        public async Task CreateAboutAsync(About about)
        {
            _context.About.Add(about);
            await _context.SaveChangesAsync();
        }

        // Update the about section
        public async Task UpdateAboutAsync(About about)
        {
            _context.About.Update(about);
            await _context.SaveChangesAsync();
        }
    }
}
