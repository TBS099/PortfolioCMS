using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Models;
using PortfolioCMS.Data;
using PortfolioCMS.Repositories.Interfaces;

namespace PortfolioCMS.Repositories.Implementations
{
    public class SocialLinkRepository : ISocialLinkRepository
    {
        private readonly AppDbContext _context;
        // Constructor to inject the AppDbContext
        public SocialLinkRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all social links
        public async Task<IEnumerable<SocialLink>> GetAllSocialLinksAsync()
        {
            return await _context.SocialLinks.ToListAsync();
        }

        // Get a social link by ID
        public async Task<SocialLink?> GetSocialLinkByIdAsync(Guid id)
        {
            return await _context.SocialLinks.FindAsync(id);
        }

        // Create a new social link
        public async Task<SocialLink> CreateSocialLinkAsync(SocialLink socialLink)
        {
            _context.SocialLinks.Add(socialLink);
            await _context.SaveChangesAsync();
            return socialLink;

        }

        // Update an existing social link
        public async Task UpdateSocialLinkAsync(SocialLink socialLink)
        {
            _context.SocialLinks.Update(socialLink);
            await _context.SaveChangesAsync();
        }

        // Delete a social link by ID
        public async Task DeleteSocialLinkAsync(Guid id)
        {
            var socialLink = await _context.SocialLinks.FindAsync(id);
            if (socialLink != null)
            {
                _context.SocialLinks.Remove(socialLink);
                await _context.SaveChangesAsync();
            }
        }
    }
}
