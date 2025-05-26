using MatrixApi.Data;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class BestellingService
    {
        private readonly AppDbContext _context;

        public BestellingService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Bestelling>> GetAllAsync()
        {
            return await _context.Bestellingen.ToListAsync();
        }

        public async Task<Bestelling?> GetByIdAsync(int id)
        {
            return await _context.Bestellingen.FindAsync(id);
        }

        public async Task AddAsync(Bestelling bestelling)
        {
            _context.Bestellingen.Add(bestelling);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bestelling bestelling)
        {
            _context.Bestellingen.Update(bestelling);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bestelling = await _context.Bestellingen.FindAsync(id);
            if (bestelling != null)
            {
                _context.Bestellingen.Remove(bestelling);
                await _context.SaveChangesAsync();
            }
        }
    }
}
