using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.Models;
using Portfolio.Repositories;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repository;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectRepository repository, ILogger<ProjectController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost("insertProject")]
        public async Task<ActionResult<Project>> InsertProject([FromBody] Project project)
        {
            if (project is null)
            {
                _logger.LogWarning("Null request body received.");
                return BadRequest("Request body must not be null.");
            }
            var created = await _repository.InsertProject(project);
            _logger.LogInformation("Project created. ProjectId: {ProjectId}. PerformedBy: {PerformedBy}", created.Id, GetAuditUserIdentity());
            return Ok(created);
        }

        [HttpPut("updateProject")]
         public async Task<ActionResult<Project>> UpdateProject([FromBody] Project project)
        {
            if (project is null)
            {
                _logger.LogWarning("Null request body received.");
                return BadRequest("Request body must not be null.");
            }

            var updatedProject = await _repository.UpdateProject(project);

            if (updatedProject is null)
            {
                _logger.LogWarning("Project not found. ProjectId: {ProjectId}", project.Id);
                return NotFound($"Project with ID {project.Id} was not found.");
            }

            _logger.LogInformation("Project updated. ProjectId: {ProjectId}. PerformedBy: {PerformedBy}", project.Id, GetAuditUserIdentity());
            return Ok(updatedProject);
        }

        [HttpGet("getAllProjects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            var projects = await _repository.GetAllProjects();
            _logger.LogInformation("Retrieved {Count} projects.", projects?.Count() ?? 0);
            return Ok(projects);
        }

        [HttpGet("getProjectById/{id}")]
        public async Task<ActionResult<Project>> GetProjectById([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Empty project ID provided.");
                return BadRequest("Project ID must not be empty.");
            }

            if (!Guid.TryParse(id, out _))
            {
                _logger.LogWarning("Invalid project ID format. ProjectId: {ProjectId}", id);
                return BadRequest("Invalid project ID format.");
            }

            var project = await _repository.GetProjectById(id);

            if (project is null)
            {
                _logger.LogWarning("Project not found. ProjectId: {ProjectId}", id);
                return NotFound($"Project with ID '{id}' was not found.");
            }

            _logger.LogInformation("Project retrieved. ProjectId: {ProjectId}", id);
            return Ok(project);
        }

        [HttpGet("getProjectsByYear/{year}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByYear([FromRoute] int year)
        {
            var projects = await _repository.GetProjectsByYear(year);
            _logger.LogInformation("Retrieved {Count} projects for year {Year}.", projects?.Count() ?? 0, year);
            return Ok(projects);
        }
      
        [HttpDelete("deleteProject/{id}")]
        public async Task<ActionResult<Project>> DeleteProject([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Empty project ID provided.");
                return BadRequest("Project ID must not be empty.");
            }

            if (!Guid.TryParse(id, out _))
            {
                _logger.LogWarning("Invalid project ID format. ProjectId: {ProjectId}", id);
                return BadRequest("Invalid project ID format.");
            }

            var deleted = await _repository.DeleteProject(id);

            if (deleted is null)
            {
                _logger.LogWarning("Project not found. ProjectId: {ProjectId}", id);
                return NotFound($"Project with ID '{id}' was not found.");
            }

            _logger.LogInformation("Project deleted. ProjectId: {ProjectId}. PerformedBy: {PerformedBy}", id, GetAuditUserIdentity());
            return Ok(deleted);
        }

        private string GetAuditUserIdentity()
        {
            var user = HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.Identity?.Name ?? user.FindFirst("sub")?.Value ?? "authenticated-user";
            }

            return "anonymous";
        }
    }
}


