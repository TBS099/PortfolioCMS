using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Authentication
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters long.")]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
