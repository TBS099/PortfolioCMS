using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces;

public interface IProjectRepository
{
    // Define the methods for CRUD operations
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<Project> CreateProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Guid id);
}