using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Project
{
    public class ProjectUpdateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? LiveUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? GithubUrl { get; set; }

        [Required]
        public string Stack { get; set; }
        public bool IsFeatured { get; set; }
    }
}
