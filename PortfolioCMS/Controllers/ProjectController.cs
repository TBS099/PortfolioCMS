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

        // Inject the service
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            var projectDTOs = projects.Select(ProjectMapper.ToDTO);
            if (!projectDTOs.Any())
                return NotFound("No projects found");
            return Ok(projectDTOs);
        }

        // GET: api/Project/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
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
            // Convert DTO → Model
            var project = ProjectMapper.ToModel(projectCreateDTO);

            // Save to database
            var created = await _projectService.CreateProjectAsync(project);

            // Return 201 Created with the new project
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
            // Find the existing project first
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound($"Project with id {id} was not found");

            // Apply the updates to the existing project
            ProjectMapper.ApplyProjectUpdate(projectUpdateDTO, project);

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

            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}

