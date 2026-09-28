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
        private readonly TurnstileService _turnstileService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(EmailService emailService, TurnstileService turnstileService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _turnstileService = turnstileService;
            _logger = logger;
        }

        // POST: api/contact
        // Public/unauthenticated — this is the portfolio's contact form.
        // No [Authorize]: visitors aren't logged in.
        [HttpPost]
        [EnableRateLimiting("contact")]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessageDTO dto)
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var verified = await _turnstileService.VerifyAsync(dto.TurnstileToken, remoteIp);
            if (!verified)
            {
                // Deliberately vague — don't tell a bot which part failed.
                return BadRequest(new { ok = false, error = "Verification failed. Please try again." });
            }

            try
            {
                await _emailService.SendContactMessageAsync(dto.Name, dto.Email, dto.Message);
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                // Log the error with the email address for debugging, but don't expose the exception details to the client.
                _logger.LogError(ex, "Failed to send contact message from {Email}.", dto.Email);
                return StatusCode(502, new { ok = false, error = "Failed to send message. Please try again later or reach out directly." });
            }
        }
    }
}