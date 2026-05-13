using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Services.Implementations
{
    public class HeroService : IHeroService
    {
        private readonly IHeroRepository _heroRepository;

        // Constructor to inject the repository
        public HeroService(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }

        // Get the hero section
        public async Task<Hero?> GetHeroAsync()
        {
            return await _heroRepository.GetHeroAsync();
        }

        // Create the hero section
        public async Task CreateHeroAsync(Hero hero)
        {
            // Set the creation and update timestamps
            hero.CreatedAt = DateTime.UtcNow;
            hero.UpdatedAt = DateTime.UtcNow;

            await _heroRepository.CreateHeroAsync(hero);
        }

        // Update the hero section
        public async Task UpdateHeroAsync(Hero hero)
        {
            hero.UpdatedAt = DateTime.UtcNow;
            await _heroRepository.UpdateHeroAsync(hero);
        }
    }
}
