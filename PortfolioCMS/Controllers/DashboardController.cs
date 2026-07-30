using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.DTOs.Dashboard;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IHeroService _heroService;
        private readonly IAboutService _aboutService;
        private readonly IExperienceService _experienceService;
        private readonly IProjectService _projectService;
        private readonly ISocialLinkService _socialLinkService;

        public DashboardController(
            IHeroService heroService,
            IAboutService aboutService,
            IExperienceService experienceService,
            IProjectService projectService,
            ISocialLinkService socialLinkService)
        {
            _heroService = heroService;
            _aboutService = aboutService;
            _experienceService = experienceService;
            _projectService = projectService;
            _socialLinkService = socialLinkService;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var hero = await _heroService.GetHeroAsync();
            var about = await _aboutService.GetAboutAsync();
            var experiences = await _experienceService.GetAllExperiencesAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var socialLinks = await _socialLinkService.GetAllSocialLinksAsync();

            var dashboard = new DashboardDTO
            {
                Hero = new SectionStatusDTO
                {
                    IsConfigured = hero != null,
                    UpdatedAt = hero?.UpdatedAt
                },
                About = new SectionStatusDTO
                {
                    IsConfigured = about != null,
                    UpdatedAt = about?.UpdatedAt
                },
                Experience = new MultiSectionStatusDTO
                {
                    Count = experiences.Count(),
                    LastUpdatedAt = experiences.Any()
                        ? experiences.Max(e => e.UpdatedAt)
                        : null
                },
                Projects = new MultiSectionStatusDTO
                {
                    Count = projects.Count(),
                    LastUpdatedAt = projects.Any()
                        ? projects.Max(p => p.UpdatedAt)
                        : null
                },
                SocialLinks = new MultiSectionStatusDTO
                {
                    Count = socialLinks.Count(),
                    LastUpdatedAt = socialLinks.Any()
                        ? socialLinks.Max(s => s.UpdatedAt)
                        : null
                }
            };

            return Ok(dashboard);
        }
    }
}