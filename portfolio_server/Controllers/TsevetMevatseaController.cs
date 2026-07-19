using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using portfolio_server.Models;
using portfolio_server.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio_server.Controllers
{
    [ApiController]
    [Route("api/tsevetmevatsea")]
    public class TsevetMevatseaController : ControllerBase
    {
        private readonly ITsevetMevatseaRepository _repository;
        private readonly ILogger<TsevetMevatseaController> _logger;

        public TsevetMevatseaController(ITsevetMevatseaRepository repository, ILogger<TsevetMevatseaController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost("insertTsevetMevatsea")]
        public async Task<ActionResult<TsevetMevatsea>> InsertTsevetMevatsea([FromBody] TsevetMevatsea tsevetMevatseaea)
        {
            var insertedTeam = await _repository.InsertTsevetMevatsea(tsevetMevatseaea);
            _logger.LogInformation("Inserted new tsevet mevatsea with IdntTsevetMevatsea: {TeamId}", insertedTeam.IdntTsevetMevatsea);
            return CreatedAtAction(nameof(GetTsevetMevatseaById), new { id = insertedTeam.IdntTsevetMevatsea }, insertedTeam);
        }

        [HttpGet("getAllTsevetMevatsea")]
        public async Task<ActionResult<IEnumerable<TsevetMevatsea>>> GetAllTsevetMevatsea()
        {
            var teams = await _repository.GetAllTsevetMevatsea();
            _logger.LogInformation("Retrieved {Count} tsevet mevatsea entries.", teams?.Count() ?? 0);
            return Ok(teams);
        }

        [HttpGet("getTsevetMevatseaById/{id:guid}")]
        public async Task<ActionResult<TsevetMevatsea>> GetTsevetMevatseaById([FromRoute] Guid id)
        {
            var team = await _repository.GetTsevetMevatseaById(id);
            if (team is null)
            {
                _logger.LogWarning("TsevetMevatsea not found. IdntTsevetMevatsea: {TeamId}", id);
                return NotFound($"TsevetMevatsea with id {id} was not found.");
            }
            return Ok(team);
        }

        [HttpPut("updateTsevetMevatseaea/{id:guid}")]
        public async Task<ActionResult> UpdateTsevetMevatsea([FromRoute] Guid id, [FromBody] TsevetMevatsea tsevetMevatseaea)
        {
            if (tsevetMevatseaea is null)
            {
                _logger.LogWarning("TsevetMevatsea update attempted with null data for id: {TeamId}", id);
                return BadRequest("TsevetMevatsea data is required.");
            }

            var updated = await _repository.UpdateTsevetMevatsea(id, tsevetMevatseaea);
            if (!updated)
            {
                _logger.LogWarning("Failed to update TsevetMevatsea. IdntTsevetMevatsea: {TeamId}", id);
                return NotFound($"TsevetMevatsea with id {id} was not found or update failed.");
            }

            _logger.LogInformation("TsevetMevatsea updated successfully. IdntTsevetMevatsea: {TeamId}", id);
            return Ok("TsevetMevatsea updated successfully.");
        }

        [HttpDelete("deleteTsevetMevatsea/{id:guid}")]
        public async Task<ActionResult> DeleteTsevetMevatsea([FromRoute] Guid id)
        {
            var deleted = await _repository.DeleteTsevetMevatsea(id);
            if (!deleted)
            {
                _logger.LogWarning("Failed to deactivate TsevetMevatsea. IdntTsevetMevatsea: {TeamId}", id);
                return NotFound($"TsevetMevatsea with id {id} was not found.");
            }

            _logger.LogInformation("TsevetMevatsea deactivated successfully. IdntTsevetMevatsea: {TeamId}", id);
            return Ok("TsevetMevatsea deactivated successfully.");
        }
    }
}
