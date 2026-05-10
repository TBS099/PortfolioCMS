using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces;

public interface IProjectRepository
{
    // Define the methods for CRUD operations
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(int id);
}