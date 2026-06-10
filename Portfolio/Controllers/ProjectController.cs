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
            var created = await _repository.InsertProject(project);
            return Created(string.Empty, created);
        }
    }
}
