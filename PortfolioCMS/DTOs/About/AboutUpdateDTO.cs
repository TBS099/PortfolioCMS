using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.About
{
    public class AboutUpdateDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required]
        public required string Body { get; set; }

        [RegularExpression(@"^$|^https?://.*", ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }

        [MaxLength(150, ErrorMessage = "Tagline cannot exceed 150 characters.")]
        public string? Tagline { get; set; }
    }
}
