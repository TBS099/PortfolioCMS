using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Hero
{
    public class HeroUpdateDTO
    {
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string ? Name { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [MaxLength(150, ErrorMessage = "Subtitle cannot exceed 150 characters.")]
        public string? Subtitle { get; set; }

        [RegularExpression(@"^$|^https?://.*", ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }
    }
}
