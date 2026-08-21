using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioCMS.DTOs.Dashboard;
using PortfolioCMS.Services.Interfaces;
using PortfolioCMS.Data;

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
        private readonly AppDbContext _context;

        // Inject all required services and context
        public DashboardController(
            IHeroService heroService,
            IAboutService aboutService,
            IExperienceService experienceService,
            IProjectService projectService,
            ISocialLinkService socialLinkService,
            AppDbContext context)
        {
            _heroService = heroService;
            _aboutService = aboutService;
            _experienceService = experienceService;
            _projectService = projectService;
            _socialLinkService = socialLinkService;
            _context = context;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // Fetch all dashboard data in parallel
            var hero = await _heroService.GetHeroAsync();
            var about = await _aboutService.GetAboutAsync();
            var experiences = await _experienceService.GetAllExperiencesAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var socialLinks = await _socialLinkService.GetAllSocialLinksAsync();
            var files = await _context.FileUploads.ToListAsync();

            // Compile dashboard status summary
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
                },
                Files = new FilesSectionDTO
                {
                    TotalCount = files.Count,
                    PrivateCount = files.Count(f => !f.IsPublic),
                    CountByCategory = files
                        .GroupBy(f => f.Category)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    LastUploadedAt = files.Any()
                        ? files.Max(f => f.UploadedAt)
                        : null
                }
            };

            return Ok(dashboard);
        }
    }
}