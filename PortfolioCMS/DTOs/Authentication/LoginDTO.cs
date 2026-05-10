using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Authentication
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
