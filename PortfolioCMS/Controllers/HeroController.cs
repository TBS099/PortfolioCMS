using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioCMS.DTOs.Hero;
using PortfolioCMS.Mappings;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeroController : ControllerBase
    {
        private readonly IHeroService _heroService;

        // Inject hero service dependency
        public HeroController(IHeroService heroService)
        {
            _heroService = heroService;
        }

        // GET: api/Hero
        [HttpGet]
        public async Task<IActionResult> GetHero()
        {
            // Fetch hero section from database
            var hero = await _heroService.GetHeroAsync();
            if (hero == null)
                return NotFound("Hero section not found");
            return Ok(HeroMapper.ToDTO(hero));
        }

        // PUT: api/Hero
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateHero([FromBody] HeroUpdateDTO heroUpdateDTO)
        {
            // Retrieve existing hero section
            var hero = await _heroService.GetHeroAsync();

            // Apply updates or create new
            if (hero == null)
            {
                // Create new hero if none exists
                hero = HeroMapper.ToModel(heroUpdateDTO);
                await _heroService.CreateHeroAsync(hero);
            }
            else
            {
                // Update existing hero with new values
                HeroMapper.ApplyHeroUpdate(heroUpdateDTO, hero);
                await _heroService.UpdateHeroAsync(hero);
            }

            return Ok(HeroMapper.ToDTO(hero));
        }
    }
}