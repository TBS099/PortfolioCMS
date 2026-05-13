using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    // Constructor to inject the repository
    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    // Get all projects, with featured projects appearing first
    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllProjectsAsync();

        // Featured projects appear first
        return projects
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt);
    }

    // Get a single project by its ID
    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        return await _projectRepository.GetProjectByIdAsync(id);
    }

    // Create a new project
    public async Task<Project> CreateProjectAsync(Project project)
    {
        // Set the creation and update timestamps
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        return await _projectRepository.CreateProjectAsync(project);
    }

    // Update an existing project
    public async Task UpdateProjectAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        await _projectRepository.UpdateProjectAsync(project);
    }

    // Delete a project by its ID
    public async Task DeleteProjectAsync(Guid id)
    {
        await _projectRepository.DeleteProjectAsync(id);
    }
}