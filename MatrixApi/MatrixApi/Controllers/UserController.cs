using System.Security.Cryptography.X509Certificates;
using MatrixApi.Data;
using MatrixApi.DTOs;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using MatrixApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatrixApi.DTOs;

namespace MatrixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);

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
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            var user = new User
            {
                Password = dto.Password,
                RoleId = dto.RoleId,
                Name = dto.Name,
                Address = dto.Address,
                Zipcode = dto.Zipcode,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var created = await _userService.AddAsync(user);
                return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.UserId) return BadRequest();

            var user = new User
            {
                UserId = dto.UserId,
                Password = dto.Password, 
                RoleId = dto.RoleId,
                Name = dto.Name,
                Address = dto.Address,
                Zipcode = dto.Zipcode,
                City = dto.City,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email
                
            };

            try
            {
                var updated = await _userService.UpdateAsync(user);
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
                var deleted = await _userService.DeleteAsync(id);
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
