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

        // Inject experience service dependency
        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        // GET: api/Experience
        [HttpGet]
        public async Task<IActionResult> GetAllExperiences()
        {
            // Fetch all experience entries
            var experiences = await _experienceService.GetAllExperiencesAsync();
            var experienceDTOs = experiences.Select(ExperienceMapper.ToDTO);
            return Ok(experienceDTOs);
        }

        // GET: api/Experience/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperienceById(Guid id)
        {
            // Fetch single experience by ID
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
            // Map DTO to model
            var experience = ExperienceMapper.ToModel(experienceCreateDTO);

            // Save new experience to database
            var created = await _experienceService.CreateExperienceAsync(experience);

            // Return created resource with location header
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
            // Retrieve existing experience
            var experience = await _experienceService.GetExperienceByIdAsync(id);

            if (experience == null)
                return NotFound($"Experience with id {id} was not found");

            // Apply updates to existing model
            ExperienceMapper.ApplyExperienceUpdate(experienceUpdateDTO, experience);

            // Save changes to database
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

            // Remove experience from database
            await _experienceService.DeleteExperienceAsync(id);
            return NoContent();
        }
    }
}