using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PortfolioCMS.DTOs.Authentication;
using PortfolioCMS.Models;
using PortfolioCMS.Services.Implementations;
using System.Net;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        // Prevent concurrent registrations
        private static readonly SemaphoreSlim _registrationLock = new(1, 1);

        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService, SignInManager<ApplicationUser> signInManager, EmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            // Lock to prevent multiple admin registrations
            await _registrationLock.WaitAsync();
            try
            {
                // Block registration if any user exists
                var hasUsers = await _userManager.Users.AnyAsync();
                if (hasUsers)
                    return Forbid();

                // Check for existing email
                var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                if (existingUser != null)
                    return BadRequest("Email is already taken.");

                // Create new user from DTO
                var user = new ApplicationUser
                {
                    UserName = registerDTO.Username,
                    Email = registerDTO.Email
                };

                // Attempt to create user with password
                var result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(errors);
                }

                // Set authentication cookie for new user
                var token = _tokenService.GenerateToken(user);
                Response.Cookies.Append("auth_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                return Ok(new { message = "Registration successful." });
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        // POST: api/Auth/Login
        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            // Verify password with lockout enabled
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            // Generate JWT token for authenticated user
            var token = _tokenService.GenerateToken(user);

            // Store token in HTTP-only cookie
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(24)
            });

            return Ok(new { message = "Login Successful" });
        }

        // POST: api/Auth/Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Remove authentication cookie
            Response.Cookies.Delete("auth_token");
            return Ok(new { message = "Logout Successful" });
        }

        // GET: api/Auth/Setup
        [HttpGet("setup")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Setup()
        {
            // Check if any admin user exists
            var hasUsers = await _userManager.Users.AnyAsync<ApplicationUser>();
            return Ok(new { requiresSetup = !hasUsers });
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

            if (user != null)
            {
                // Generate reset token for user
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var frontendUrl = _configuration["FrontendUrl"]
                    ?? "http://localhost:5173";

                // Build password reset link
                var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&userId={user.Id}";

                // Send reset link via email
                await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink);
            }

            return Ok(new { message = "If that email exists, a reset link has been sent." });
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId);

            if (user == null)
                return BadRequest("Invalid request.");

            // Reset user password with token
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest($"Failed to reset password: {string.Join(", ", errors)}");
            }

            return Ok(new { message = "Password reset successful" });
        }

        // GET: api/Auth/me
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            return Ok(new { message = "Authenticated" });
        }
    }
}