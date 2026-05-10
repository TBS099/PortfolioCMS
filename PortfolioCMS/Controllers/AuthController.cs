using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PortfolioCMS.Models;
using PortfolioCMS.DTOs.Authentication;
using PortfolioCMS.Services.Implementations;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
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
            return Ok(new { token });
        }
    }
}
