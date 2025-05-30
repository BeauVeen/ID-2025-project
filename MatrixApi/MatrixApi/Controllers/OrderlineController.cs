using System.Security.Cryptography.X509Certificates;
using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using MatrixApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderlineController : ControllerBase
    {
        private readonly OrderlineService _orderlineService;

        public OrderlineController(OrderlineService orderlineService)
        {
            _orderlineService = orderlineService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderline>>> GetAll()
        {
            try
            {
                var orderlines = await _orderlineService.GetAllAsync();
                return Ok(orderlines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Orderline>> GetById(int id)
        {
            try
            {
                var orderline = await _orderlineService.GetByIdAsync(id);
                return Ok(orderline);

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
        public async Task<ActionResult<Orderline>> Create(Orderline orderline)
        {
            try
            {
                var created = await _orderlineService.AddAsync(orderline);
                return CreatedAtAction(nameof(GetById), new { id = created.OrderlineId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Orderline orderline)
        {
            if (id != orderline.OrderlineId) return BadRequest();

            try
            {
                var updated = await _orderlineService.UpdateAsync(orderline);
                if (!updated)
                {
                    return NotFound();
                }
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _orderlineService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
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
