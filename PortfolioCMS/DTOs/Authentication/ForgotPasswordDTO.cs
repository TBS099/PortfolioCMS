using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Authentication
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}