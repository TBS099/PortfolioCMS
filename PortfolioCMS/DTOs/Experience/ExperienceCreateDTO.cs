using PortfolioCMS.Models;
using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Experience
{
    public class ExperienceCreateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Organization cannot exceed 100 characters.")]
        public required string Organization { get; set; }

        [Required]
        public required ExperienceType Type { get; set; }

        [Required]
        public required DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string? Location { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }
}
