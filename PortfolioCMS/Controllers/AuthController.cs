using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            // Check if the email is taken
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
                return BadRequest("Email is already taken.");

            // Create a new user
            var user = new ApplicationUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                // Return the errors if user creation failed
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest($"Failed to register user: {errors}");
            }

            // Generate a token for the newly registered user
            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }

        // POST: api/Auth/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            // Check the password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            // Generate a token for the authenticated user
            var token = _tokenService.GenerateToken(user);

            // Set the token in an HTTP-only cookie
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(24) // Set cookie expiration as needed
            });

            return Ok(new { message = "Login Successful" });
        }

        // POST: api/Auth/Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear the authentication cookie
            Response.Cookies.Delete("auth_token");
            return Ok(new { message = "Logout Successful" });
        }

        // GET: api/Auth/Setup
        [HttpGet("setup")]
        public async Task<IActionResult> Setup()
        {
            // Check if the admin user already exists
            var hasUsers = await _userManager.Users.AnyAsync<ApplicationUser>();
            return Ok(new { requiresSetup = !hasUsers });
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var frontendUrl = _configuration["FrontendUrl"]
                    ?? "http://localhost:5173";

                var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={user.Email}";

                await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink);
            }

            return Ok(new { message = "If that email exists, a reset link has been sent." });
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
                return BadRequest("No user found with the provided email.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest($"Failed to reset password: {errors}");
            }

            return Ok(new { message = "Password reset successful" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            return Ok(new { message = "Authenticated" });
        }
    }
}
