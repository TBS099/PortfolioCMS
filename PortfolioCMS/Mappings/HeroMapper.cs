using PortfolioCMS.DTOs.Hero;
using PortfolioCMS.Models;

namespace PortfolioCMS.Mappings
{
    public static class HeroMapper
    {
        // Returning to the frontend
        public static HeroDTO ToDTO(Hero hero)
        {
            return new HeroDTO
            {
                Name = hero.Name,
                Title = hero.Title,
                Subtitle = hero.Subtitle,
                ImageUrl = hero.ImageUrl
            };
        }

        // Saving to the database
        public static Hero ToModel(HeroUpdateDTO dto)
        {
            return new Hero
            {
                Name = dto.Name,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                ImageUrl = dto.ImageUrl
            };
        }

        // Updating in the database
        public static void ApplyHeroUpdate(HeroUpdateDTO dto, Hero hero)
        {
            hero.Name = dto.Name;
            hero.Title = dto.Title;
            hero.Subtitle = dto.Subtitle;
            hero.ImageUrl = dto.ImageUrl;
        }
    }
}
