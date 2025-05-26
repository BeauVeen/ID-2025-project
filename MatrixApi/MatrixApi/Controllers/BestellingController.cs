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
    public class BestellingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BestellingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bestelling>>> GetAll()
        {
            return await _context.Bestellingen.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bestelling>> GetById(int id)
        {
            var Bestelling = await _context.Bestellingen.FindAsync(id);
            if (Bestelling == null) return NotFound();
            return Bestelling;
        }

        [HttpPost]
        public async Task<ActionResult<Bestelling>> Create(Bestelling bestelling)
        {
            _context.Bestellingen.Add(bestelling);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bestelling.BestelId }, bestelling);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Bestelling bestelling)
        {
            if (id != bestelling.BestelId) 
                return BadRequest();

            _context.Entry(bestelling).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bestellingen.Any(e => e.BestelId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        { 
            var bestelling = await _context.Bestellingen.FindAsync(id);
            if (bestelling == null)
                return NotFound();

            _context.Bestellingen.Remove(bestelling);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
