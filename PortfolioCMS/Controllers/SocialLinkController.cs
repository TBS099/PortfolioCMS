using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.DTOs.SocialLink;
using PortfolioCMS.Mappings;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocialLinkController : ControllerBase
    {
        private readonly ISocialLinkService _socialLinkService;

        // Inject the service
        public SocialLinkController(ISocialLinkService socialLinkService)
        {
            _socialLinkService = socialLinkService;
        }

        // GET: api/SocialLink
        [HttpGet]
        public async Task<IActionResult> GetAllSocialLinks()
        {
            var socialLinks = await _socialLinkService.GetAllSocialLinksAsync();
            var socialLinkDTOs = socialLinks.Select(SocialLinkMapper.ToDTO);
            return Ok(socialLinkDTOs);
        }

        // GET: api/SocialLink/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSocialLinkById(Guid id)
        {
            var socialLink = await _socialLinkService.GetSocialLinkByIdAsync(id);
            if (socialLink == null)
                return NotFound($"Social link with id {id} was not found");
            return Ok(SocialLinkMapper.ToDTO(socialLink));
        }

        // POST: api/SocialLink
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSocialLink([FromBody] SocialLinkCreateDTO socialLinkCreateDTO)
        {
            // Convert DTO → Model
            var socialLink = SocialLinkMapper.ToModel(socialLinkCreateDTO);

            // Call the service to create the social link in the database
            var created = await _socialLinkService.CreateSocialLinkAsync(socialLink);

            // Return the created social link as a DTO with a 201 Created status
            return CreatedAtAction(
                nameof(GetSocialLinkById),
                new { id = created.Id },
                SocialLinkMapper.ToDTO(created)
            );
        }

        // PUT: api/SocialLink/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSocialLink(Guid id, [FromBody] SocialLinkUpdateDTO socialLinkUpdateDTO)
        {
            // Check if the social link exists
            var existing = await _socialLinkService.GetSocialLinkByIdAsync(id);
            if (existing == null)
                return NotFound($"Social link with id {id} was not found");

            // Apply updates from the DTO to the existing model
            SocialLinkMapper.ApplySocialLinkUpdate(socialLinkUpdateDTO, existing);

            // Call the service to update the social link in the database
            await _socialLinkService.UpdateSocialLinkAsync(existing);
            return Ok(SocialLinkMapper.ToDTO(existing));
        }

        // DELETE: api/SocialLink/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSocialLinkAsync(Guid id)
        {
            var socialLink = await _socialLinkService.GetSocialLinkByIdAsync(id);

            // Check if the social link exists
            if (socialLink == null)
                return NotFound($"Social link with id {id} was not found");

            await _socialLinkService.DeleteSocialLinkAsync(id);
            return NoContent();

        }
    }
}
