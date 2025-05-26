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
    public class BestelregelController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BestelregelController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bestelregel>>> GetAll()
        {
            return await _context.Bestelregels.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bestelregel>> GetById(int id)
        {
            var bestelregel = await _context.Bestelregels.FindAsync(id);
            if (bestelregel == null) return NotFound();
            return bestelregel;
        }

        [HttpPost]
        public async Task<ActionResult<Bestelregel>> Create(Bestelregel bestelregel)
        {
            _context.Bestelregels.Add(bestelregel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bestelregel.BestelregelId }, bestelregel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Bestelregel bestelregel)
        {
            if (id != bestelregel.BestelregelId) 
                return BadRequest();

            _context.Entry(bestelregel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bestelregels.Any(e => e.BestelregelId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        { 
            var bestelregel = await _context.Bestelregels.FindAsync(id);
            if (bestelregel == null)
                return NotFound();

            _context.Bestelregels.Remove(bestelregel);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
