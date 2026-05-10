using PortfolioCMS.DTOs.Project;
using PortfolioCMS.Models;

namespace PortfolioCMS.Mappings
{
    public static class ProjectMapper
    {
        // Returning to the frontend
        public static ProjectDTO ToDTO(Project project)
        {
            return new ProjectDTO
            {
                Id = project.Id,
                Title = project.Title,
                Slug = project.Slug,
                Description = project.Description,
                ImageUrl = project.ImageUrl,
                LiveUrl = project.LiveUrl,
                GithubUrl = project.GithubUrl,
                Stack = project.Stack,
                IsFeatured = project.IsFeatured
            };
        }

        // Saving to the database
        public static Project ToModel(ProjectCreateDTO dto)
        {             
            return new Project
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                LiveUrl = dto.LiveUrl,
                GithubUrl = dto.GithubUrl,
                Stack = dto.Stack,
                IsFeatured = dto.IsFeatured
            };
        }

        // Updating in the database
        public static void ApplyUpdate(ProjectUpdateDTO dto, Project project)
        {
            project.Title = dto.Title;
            project.Description = dto.Description;
            project.ImageUrl = dto.ImageUrl;
            project.LiveUrl = dto.LiveUrl;
            project.GithubUrl = dto.GithubUrl;
            project.Stack = dto.Stack;
            project.IsFeatured = dto.IsFeatured;
        }
    }
}
