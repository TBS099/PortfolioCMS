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
        
        // Inject the service
        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        // GET: api/About
        [HttpGet]
        public async Task<IActionResult> GetAbout()
        {
            var about = await _aboutService.GetAboutAsync();
            if (about == null)
                return NotFound("About section not found");
            return Ok(AboutMapper.ToDTO(about));
        }

        // PUT: api/About
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateAbout([FromBody] AboutUpdateDTO aboutUpdateDTO)
        {
            // Get existing about
            var about = await _aboutService.GetAboutAsync();
            // Apply updates
            if (about == null)
            {
                // Create new about if it doesn't exist
                about = AboutMapper.ToModel(aboutUpdateDTO);
                await _aboutService.CreateAboutAsync(about);
            }
            else
            {
                // Update existing about
                AboutMapper.ApplyAboutUpdate(aboutUpdateDTO, about);
                await _aboutService.UpdateAboutAsync(about);
            }
            return Ok(AboutMapper.ToDTO(about));
        }
    }
}
