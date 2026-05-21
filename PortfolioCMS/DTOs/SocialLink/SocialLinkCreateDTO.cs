using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.SocialLink
{
    public class SocialLinkCreateDTO
    {
        [Required]
        [StringLength(100)]
        public required string Platform { get; set; }

        [Required]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public required string Url { get; set; }
    }
}
