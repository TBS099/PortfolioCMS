using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;

namespace PortfolioCMS.Repositories.Implementations
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public ExperienceRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all experiences
        public async Task<IEnumerable<Experience>> GetAllExperiencesAsync()
        {
            return await _context.Experiences.ToListAsync();
        }

        // Get an experience by ID
        public async Task<Experience?> GetExperienceByIdAsync(Guid id)
        {
            return await _context.Experiences.FindAsync(id);
        }

        // Create a new experience
        public async Task<Experience> CreateExperienceAsync(Experience experience)
        {
            _context.Experiences.Add(experience);
            return experience;
        }

        // Update an existing experience
        public async Task UpdateExperienceAsync(Experience experience)
        {
            _context.Experiences.Update(experience);
            await _context.SaveChangesAsync();
        }

        // Delete an experience by ID
        public async Task DeleteExperienceAsync(Guid id)
        {
            var experience = await _context.Experiences.FindAsync(id);
            if (experience != null)
            {
                _context.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
            }
        }
    }
}
