using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces
{
    public interface ISocialLinkRepository
    {
        // Define the methods for CRUD operations
        public Task<IEnumerable<SocialLink>> GetAllSocialLinksAsync();
        public Task<SocialLink?> GetSocialLinkByIdAsync(Guid id);
        public Task<SocialLink> CreateSocialLinkAsync(SocialLink socialLink);
        public Task UpdateSocialLinkAsync(SocialLink socialLink);
        public Task DeleteSocialLinkAsync(Guid id);
    }
}
