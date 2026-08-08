using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PortfolioCMS.Models;

namespace PortfolioCMS.Services.Implementations
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user)
        {
            // Get jwt settings from appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json.");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // Safely parse ExpiryHours to avoid passing a possible null to int.Parse
            var expiryHoursValue = jwtSettings["ExpiryHours"];
            if (!int.TryParse(expiryHoursValue, out var expiryHours))
            {
                throw new InvalidOperationException("JWT ExpiryHours is not configured or is not a valid integer in appsettings.json.");
            }

            // Create claims based on the user information for the token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("security_stamp", user.SecurityStamp ?? string.Empty)
            };

            // Sign the token using the secret key and specify the signing algorithm
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: creds
            );

            // Return the serialized token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
