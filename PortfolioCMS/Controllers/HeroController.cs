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

        // Inject the service
        public HeroController(IHeroService heroService)
        {
            _heroService = heroService;
        }

        // GET: api/Hero
        [HttpGet]
        public async Task<IActionResult> GetHero()
        {
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
            // Get existing hero
            var hero = await _heroService.GetHeroAsync();

            // Apply updates
            if (hero == null)
            {
                // Create new hero if it doesn't exist
                hero = HeroMapper.ToModel(heroUpdateDTO);
                await _heroService.CreateHeroAsync(hero);
            }
            else
            {
                // Update existing hero
                HeroMapper.ApplyHeroUpdate(heroUpdateDTO, hero);
                await _heroService.UpdateHeroAsync(hero);
            }

            return Ok(HeroMapper.ToDTO(hero));
        }
    }
}
