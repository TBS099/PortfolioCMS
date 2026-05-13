using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;


namespace PortfolioCMS.Repositories.Implementations
{
    public class HeroRepository : IHeroRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public HeroRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get Hero Section
        public async Task<Hero?> GetHeroAsync()
        {
            return await _context.Hero.FirstOrDefaultAsync();
        }

        // Create Hero Section
        public async Task CreateHeroAsync(Hero hero)
        {
            _context.Hero.Add(hero);
            await _context.SaveChangesAsync();
        }

        // Update Hero Section
        public async Task UpdateHeroAsync(Hero hero)
        {
            _context.Hero.Update(hero);
            await _context.SaveChangesAsync();
        }
    }
}
