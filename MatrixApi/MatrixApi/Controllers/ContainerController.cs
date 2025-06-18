using MatrixApi.DTOs;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using MatrixApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatrixApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly ContainerService _containerService;

        public ContainerController(ContainerService containerService)
        {
            _containerService = containerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Container>>> GetAll()
        {
            try
            {
                var containers = await _containerService.GetAllAsync();
                return Ok(containers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Container>> GetById(int id)
        {
            try
            {
                var container = await _containerService.GetByIdAsync(id);
                return Ok(container);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Container>> Create(ContainerCreateDto dto)
        {
            try
            {
                var created = await _containerService.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ContainerId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Container>> UpdateStatus(int id, [FromBody] ContainerUpdateDto dto)
        {
            try
            {
                var updated = await _containerService.UpdateStatusAsync(id, dto.Status);
                return Ok(updated);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _containerService.DeleteAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
