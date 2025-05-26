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
    public class CategorieController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategorieController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categorie>>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categorie>> GetById(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null) return NotFound();
            return categorie;
        }

        [HttpPost]
        public async Task<ActionResult<Categorie>> Create(Categorie categorie)
        {
            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = categorie.CategorieId }, categorie);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Categorie categorie)
        {
            if (id != categorie.CategorieId) 
                return BadRequest();

            _context.Entry(categorie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.CategorieId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        { 
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
                return NotFound();

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
