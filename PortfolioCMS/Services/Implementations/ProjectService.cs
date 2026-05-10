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
        var projects = await _projectRepository.GetAllAsync();

        // Featured projects appear first
        return projects
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt);
    }

    // Get a single project by its ID
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _projectRepository.GetByIdAsync(id);
    }

    // Create a new project
    public async Task<Project> CreateProjectAsync(Project project)
    {
        // Set the creation and update timestamps
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        return await _projectRepository.CreateAsync(project);
    }

    // Update an existing project
    public async Task UpdateProjectAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        await _projectRepository.UpdateAsync(project);
    }

    // Delete a project by its ID
    public async Task DeleteProjectAsync(int id)
    {
        await _projectRepository.DeleteAsync(id);
    }
}