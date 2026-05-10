using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;

namespace PortfolioCMS.Repositories.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    // Constructor to inject the AppDbContext
    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    // Get all projects
    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects.ToListAsync();
    }

    // Get a project by ID
    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FindAsync(id);
    }

    // Create a new project
    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    // Update an existing project
    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    // Delete a project by ID
    public async Task DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}