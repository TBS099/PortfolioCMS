using PortfolioCMS.DTOs.SocialLink;
using PortfolioCMS.Models;

namespace PortfolioCMS.Mappings
{
    public static class SocialLinkMapper
    {
        // Returning to the frontend
        public static SocialLinkDTO ToDTO(SocialLink socialLink)
        {
            return new SocialLinkDTO
            {
                Id = socialLink.Id,
                Platform = socialLink.Platform,
                Url = socialLink.Url
            };
        }

        // Saving to the database
        public static SocialLink ToModel(SocialLinkCreateDTO dto)
        {
            return new SocialLink
            {
                Platform = dto.Platform,
                Url = dto.Url
            };
        }

        // Updating in the database
        public static void ApplySocialLinkUpdate(SocialLinkUpdateDTO dto, SocialLink socialLink)
        {
            socialLink.Platform = dto.Platform;
            socialLink.Url = dto.Url;
        }
    }
}
