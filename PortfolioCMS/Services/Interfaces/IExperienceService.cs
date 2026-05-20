using PortfolioCMS.Models;

namespace PortfolioCMS.Services.Interfaces
{
    public interface IExperienceService
    {
        // Define the methods for CRUD operations
        Task<IEnumerable<Experience>> GetAllExperiencesAsync();
        Task<Experience?> GetExperienceByIdAsync(Guid id);
        Task<Experience> CreateExperienceAsync(Experience experience);
        Task UpdateExperienceAsync(Experience experience);
        Task DeleteExperienceAsync(Guid id);
    }
}
