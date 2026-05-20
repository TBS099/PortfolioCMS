using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces
{
    public interface IExperienceRepository
    {
        // Define the methods for CRUD operations
        Task <IEnumerable<Experience>> GetAllExperiencesAsync();
        Task<Experience?> GetExperienceByIdAsync(Guid id);
        Task <Experience> CreateExperienceAsync(Experience experience);
        Task UpdateExperienceAsync(Experience experience);
        Task DeleteExperienceAsync(Guid id);
    }
}
