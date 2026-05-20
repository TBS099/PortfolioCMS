using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Services.Implementations
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _experienceRepository;

        // Constructor to inject the repository
        public ExperienceService(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        // Get all experiences
        public async Task<IEnumerable<Experience>> GetAllExperiencesAsync()
        {
            var experiences = await _experienceRepository.GetAllExperiencesAsync();
            return experiences.OrderByDescending(e => e.StartDate);
        }

        // Get a single experience by its ID
        public async Task<Experience?> GetExperienceByIdAsync(Guid id)
        {
            return await _experienceRepository.GetExperienceByIdAsync(id);
        }

        // Create a new experience
        public async Task<Experience> CreateExperienceAsync(Experience experience)
        {
            // Set the creation and update timestamps
            experience.CreatedAt = DateTime.UtcNow;
            experience.UpdatedAt = DateTime.UtcNow;

            return await _experienceRepository.CreateExperienceAsync(experience);
        }

        // Update an existing experience
        public async Task UpdateExperienceAsync(Experience experience)
        {
            experience.UpdatedAt = DateTime.UtcNow;
            await _experienceRepository.UpdateExperienceAsync(experience);
        }

        // Delete an experience by its ID
        public async Task DeleteExperienceAsync(Guid id)
        {
            await _experienceRepository.DeleteExperienceAsync(id);
        }
    }
}
