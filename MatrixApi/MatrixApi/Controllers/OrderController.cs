using System.Security.Cryptography.X509Certificates;
using MatrixApi.Data;
using MatrixApi.DTOs;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using MatrixApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);

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
        public async Task<ActionResult<Order>> Create([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                byte[]? signatureBytes = null;
                if (orderDto.Signature != null)
                {
                    using var ms = new MemoryStream();
                    await orderDto.Signature.CopyToAsync(ms);
                    signatureBytes = ms.ToArray();
                    Console.WriteLine($"Signature bytes length: {signatureBytes.Length}");
                }
                else
                {
                    Console.WriteLine("No signature received");
                }

                var order = new Order
                {
                    UserId = orderDto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Status = orderDto.Status,
                    Signature = signatureBytes,
                    Orderlines = orderDto.Orderlines.Select(ol => new Orderline
                    {
                        ProductId = ol.ProductId,
                        Amount = ol.Amount,
                        Price = ol.Price
                    }).ToList()
                };

                var created = await _orderService.AddAsync(order);
                return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] OrderUpdateDto dto)
        {
            if (id != dto.OrderId) return BadRequest();

            try
            {
                byte[]? signatureBytes = null;

                if (dto.Signature != null)
                {
                    using var ms = new MemoryStream();
                    await dto.Signature.CopyToAsync(ms);
                    signatureBytes = ms.ToArray();
                }

                var order = new Order
                {
                    OrderId = dto.OrderId,
                    UserId = dto.UserId,
                    Status = dto.Status,
                    Signature = signatureBytes,
                };

                var updated = await _orderService.UpdateAsync(order);

                if (!updated)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _orderService.DeleteAsync(id);
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
