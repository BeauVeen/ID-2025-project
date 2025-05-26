using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatrixApi.Models;
using MatrixApi.Services;
using System.Security.Cryptography.X509Certificates;
using MatrixApi.Data;

namespace MatrixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GebruikerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GebruikerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetAll()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Gebruiker>> GetById(int id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker == null) return NotFound();
            return gebruiker;
        }

        [HttpPost]
        public async Task<ActionResult<Gebruiker>> Create(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = gebruiker.UserId }, gebruiker);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Gebruiker gebruiker)
        {
            if (id != gebruiker.UserId) 
                return BadRequest();

            _context.Entry(gebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Gebruikers.Any(e => e.UserId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        { 
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker == null)
                return NotFound();

            _context.Gebruikers.Remove(gebruiker);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
