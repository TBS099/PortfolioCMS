using PortfolioCMS.DTOs.About;
using PortfolioCMS.Models;

namespace PortfolioCMS.Mappings
{
    public static class AboutMapper
    {
        // Returning to the frontend
        public static AboutDTO ToDTO(About about)
        {
            return new AboutDTO
            {
                Title = about.Title,
                Body = about.Body,
                ImageUrl = about.ImageUrl,
                Tagline = about.Tagline
            };
        }

        // Saving to the database
        public static About ToModel(AboutUpdateDTO dto)
        {
            return new About
            {
                Title = dto.Title,
                Body = dto.Body,
                ImageUrl = dto.ImageUrl,
                Tagline = dto.Tagline
            };
        }

        // Updating in the database
        public static void ApplyAboutUpdate(AboutUpdateDTO dto, About about)
        {
            about.Title = dto.Title;
            about.Body = dto.Body;
            about.ImageUrl = dto.ImageUrl;
            about.Tagline = dto.Tagline;
        }
    }
}
