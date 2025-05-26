using MatrixApi.Data;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class GebruikerService
    {
        private readonly AppDbContext _context;

        public GebruikerService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Gebruiker>> GetAllAsync()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        public async Task<Gebruiker?> GetByIdAsync(int id)
        {
            return await _context.Gebruikers.FindAsync(id);
        }

        public async Task AddAsync(Gebruiker gebruiker)
        {
            _context.Gebruikers.Add(gebruiker);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Gebruiker gebruiker)
        {
            _context.Gebruikers.Update(gebruiker);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker != null)
            {
                _context.Gebruikers.Remove(gebruiker);
                await _context.SaveChangesAsync();
            }
        }
    }
}
