using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioCMS.DTOs.About;
using PortfolioCMS.Mappings;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;

        // Inject about service dependency
        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        // GET: api/About
        [HttpGet]
        public async Task<IActionResult> GetAbout()
        {
            // Fetch about section from database
            var about = await _aboutService.GetAboutAsync();
            if (about == null)
                return NotFound("About section not found");
            // Return mapped DTO response
            return Ok(AboutMapper.ToDTO(about));
        }

        // PUT: api/About
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateAbout([FromBody] AboutUpdateDTO aboutUpdateDTO)
        {
            // Retrieve existing about section
            var about = await _aboutService.GetAboutAsync();
            if (about == null)
            {
                // Create new if none exists
                about = AboutMapper.ToModel(aboutUpdateDTO);
                await _aboutService.CreateAboutAsync(about);
            }
            else
            {
                // Update existing with new values
                AboutMapper.ApplyAboutUpdate(aboutUpdateDTO, about);
                await _aboutService.UpdateAboutAsync(about);
            }
            // Return updated about data
            return Ok(AboutMapper.ToDTO(about));
        }
    }
}