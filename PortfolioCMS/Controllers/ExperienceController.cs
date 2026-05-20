using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.DTOs.Experience;
using PortfolioCMS.Mappings;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        // Inject the service
        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        // GET: api/Experience
        [HttpGet]
        public async Task<IActionResult> GetAllExperiences()
        {
            var experiences = await _experienceService.GetAllExperiencesAsync();
            var experienceDTOs = experiences.Select(ExperienceMapper.ToDTO);
            return Ok(experienceDTOs);
        }

        // GET: api/Experience/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperienceById(Guid id)
        {
            var experience = await _experienceService.GetExperienceByIdAsync(id);

            if (experience == null)
                return NotFound($"Experience with id {id} was not found");

            return Ok(ExperienceMapper.ToDTO(experience));
        }

        // POST: api/Experience
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateExperience([FromBody] ExperienceCreateDTO experienceCreateDTO)
        {
            // Convert DTO → Model
            var experience = ExperienceMapper.ToModel(experienceCreateDTO);

            // Save to database
            var created = await _experienceService.CreateExperienceAsync(experience);

            // Return 201 Created with the new experience
            return CreatedAtAction(
                nameof(GetExperienceById),
                new { id = created.Id },
                ExperienceMapper.ToDTO(created)
            );
        }

        // PUT: api/Experience/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] ExperienceUpdateDTO experienceUpdateDTO)
        {
            // Fetch existing experience from database
            var experience = await _experienceService.GetExperienceByIdAsync(id);

            if (experience == null)
                return NotFound($"Experience with id {id} was not found");

            // Apply updates from DTO to the existing model
            ExperienceMapper.ApplyExperienceUpdate(experienceUpdateDTO, experience);

            await _experienceService.UpdateExperienceAsync(experience);
            return Ok(ExperienceMapper.ToDTO(experience));
        }

        // DELETE: api/Experience/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(Guid id)
        {
            var experience = await _experienceService.GetExperienceByIdAsync(id);

            if (experience == null)
                return NotFound($"Experience with id {id} was not found");

            await _experienceService.DeleteExperienceAsync(id);
            return NoContent();
        }
    }
}
