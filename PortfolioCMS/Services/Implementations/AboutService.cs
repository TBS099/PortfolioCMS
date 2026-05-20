using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Services.Implementations
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;

        // Constructor to inject the repository
        public AboutService(IAboutRepository aboutRepository)
        {
            _aboutRepository = aboutRepository;
        }

        // Get the about section
        public async Task<About?> GetAboutAsync()
        {
            return await _aboutRepository.GetAboutAsync();
        }

        // Create the about section
        public Task CreateAboutAsync(About about)
        {
            // Set the creation and update timestamps
            about.CreatedAt = DateTime.UtcNow;
            about.UpdatedAt = DateTime.UtcNow;
            return _aboutRepository.CreateAboutAsync(about);
        }

        // Update the about section
        public async Task UpdateAboutAsync(About about)
        {
            about.UpdatedAt = DateTime.UtcNow;
            await _aboutRepository.UpdateAboutAsync(about);
        }
    }
}
