using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PortfolioCMS.DTOs.Contact;
using PortfolioCMS.Services.Implementations;

namespace PortfolioCMS.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(EmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        // POST: api/contact
        // Public/unauthenticated — this is the portfolio's contact form.
        // No [Authorize]: visitors aren't logged in.
        [HttpPost]
        [EnableRateLimiting("contact")]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessageDTO dto)
        {
            try
            {
                await _emailService.SendContactMessageAsync(dto.Name, dto.Email, dto.Message);
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                // Contact-form failures are surfaced to the caller (unlike
                // password-reset emails, which intentionally fail silently
                // for security reasons) — the frontend shows "Transmission
                // failed" on a non-2xx response, so swallowing this here
                // would make submissions silently vanish instead.
                _logger.LogError(ex, "Failed to send contact message from {Email}.", dto.Email);
                return StatusCode(502, new { ok = false, error = "Failed to send message. Please try again later or reach out directly." });
            }
        }
    }
}