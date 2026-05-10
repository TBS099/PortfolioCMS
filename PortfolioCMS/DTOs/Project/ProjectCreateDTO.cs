using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Project
{
    public class ProjectCreateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase, numbers and hyphens only.")]
        public string Slug { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "LiveUrl must be a valid URL.")]
        public string? LiveUrl { get; set; }

        [Url(ErrorMessage = "GithubUrl must be a valid URL.")]
        public string? GithubUrl { get; set; }

        [Required]
        public string Stack { get; set; }
        public bool IsFeatured { get; set; }
    }
}
