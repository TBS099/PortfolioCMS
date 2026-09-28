using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.DTOs.Contact
{
    // Matches exactly what the portfolio frontend sends from sendContactMessage()
    // in src/lib/api/cms.js: { name, email, message, turnstileToken }.
    public class ContactMessageDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(254)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")]
        public required string Message { get; set; }

        // The Turnstile widget's single-use response token, verified
        // server-side against Cloudflare before the email is sent.
        [Required]
        [MaxLength(2048)]
        public required string TurnstileToken { get; set; }
    }
}