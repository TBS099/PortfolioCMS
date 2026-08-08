using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Authentication
{
    public class ResetPasswordDTO
    {
        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters long.")]
        public required string NewPassword { get; set; }
    }
}