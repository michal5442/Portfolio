using Microsoft.AspNetCore.Mvc;
using Portfolio.Entities;
using Portfolio.Repositories;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repository;

        public ProjectController(IProjectRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<AviationProject>> Create([FromBody] AviationProject project)
        {
             // להוסיף בדיקות על השדות שנכנסו...
            var created = await _repository.InsertProject(project);
            if (created == null)
            {
                return BadRequest("Failed to create project.");
            }
            return Ok(created);
        }
    }
}
