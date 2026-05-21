using PortfolioCMS.Models;
using PortfolioCMS.Repositories.Interfaces;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Services.Implementations
{
    public class SocialLinkService : ISocialLinkService
    {
        private readonly ISocialLinkRepository _socialLinkRepository;

        // Constructor to inject the repository
        public SocialLinkService(ISocialLinkRepository socialLinkRepository)
        {
            _socialLinkRepository = socialLinkRepository;
        }

        // Get all social links
        public async Task<IEnumerable<SocialLink>> GetAllSocialLinksAsync()
        {
            return await _socialLinkRepository.GetAllSocialLinksAsync();
        }

        // Get a single social link by its ID
        public async Task<SocialLink?> GetSocialLinkByIdAsync(Guid id)
        {
            return await _socialLinkRepository.GetSocialLinkByIdAsync(id);
        }

        // Create a new social link
        public async Task<SocialLink> CreateSocialLinkAsync(SocialLink socialLink)
        {
            socialLink.CreatedAt = DateTime.UtcNow;
            socialLink.UpdatedAt = DateTime.UtcNow;
            return await _socialLinkRepository.CreateSocialLinkAsync(socialLink);
        }

        // Update an existing social link
        public async Task UpdateSocialLinkAsync(SocialLink socialLink)
        {
            socialLink.UpdatedAt = DateTime.UtcNow;
            await _socialLinkRepository.UpdateSocialLinkAsync(socialLink);
        }

        // Delete a social link by its ID
        public async Task DeleteSocialLinkAsync(Guid id)
        {
            await _socialLinkRepository.DeleteSocialLinkAsync(id);
        }
    }
}
