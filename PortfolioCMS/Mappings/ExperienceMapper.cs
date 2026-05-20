using PortfolioCMS.DTOs.Experience;
using PortfolioCMS.Models;

namespace PortfolioCMS.Mappings
{
    public static class ExperienceMapper
    {
        // Returning to the frontend
        public static ExperienceDTO ToDTO(Experience experience)
        {
            return new ExperienceDTO
            {
                Id = experience.Id,
                Title = experience.Title,
                Organization = experience.Organization,
                Type = experience.Type,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Location = experience.Location,
                Description = experience.Description
            };
        }

        // Saving to the database
        public static Experience ToModel(ExperienceCreateDTO dto)
        {
            return new Experience
            {
                Title = dto.Title,
                Organization = dto.Organization,
                Type = dto.Type,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location= dto.Location,
                Description = dto.Description
            };
        }

        // Updating in the database
        public static void ApplyExperienceUpdate(ExperienceUpdateDTO dto, Experience experience)
        {
            experience.Title = dto.Title;
            experience.Organization = dto.Organization;
            experience.Type = dto.Type;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.EndDate;
            experience.Location = dto.Location;
            experience.Description = dto.Description;
        }
    }
}
