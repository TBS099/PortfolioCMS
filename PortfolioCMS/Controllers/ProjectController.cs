using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.DTOs.Project;
using PortfolioCMS.Mappings;
using PortfolioCMS.Services.Interfaces;

namespace PortfolioCMS.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        // Inject project service dependency
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            // Fetch all projects from database
            var projects = await _projectService.GetAllProjectsAsync();
            var projectDTOs = projects.Select(ProjectMapper.ToDTO);
            return Ok(projectDTOs);
        }

        // GET: api/Project/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            // Fetch single project by ID
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound($"Project with id {id} was not found");

            return Ok(ProjectMapper.ToDTO(project));
        }

        // POST: api/Project
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO projectCreateDTO)
        {
            // Map DTO to model
            var project = ProjectMapper.ToModel(projectCreateDTO);

            // Save new project to database
            var created = await _projectService.CreateProjectAsync(project);

            // Return created resource with location header
            return CreatedAtAction(
                nameof(GetProjectById),
                new { id = created.Id },
                ProjectMapper.ToDTO(created)
            );
        }

        // PUT: api/Project/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectUpdateDTO projectUpdateDTO)
        {
            // Retrieve existing project
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound($"Project with id {id} was not found");

            // Apply updates to existing model
            ProjectMapper.ApplyProjectUpdate(projectUpdateDTO, project);

            // Save changes to database
            await _projectService.UpdateProjectAsync(project);
            return Ok(ProjectMapper.ToDTO(project));
        }

        // DELETE: api/Project/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound($"Project with id {id} was not found");

            // Remove project from database
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}