using MatrixApi.Data;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class BestelregelService
    {
        private readonly AppDbContext _context;

        public BestelregelService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Bestelregel>> GetAllAsync()
        {
            return await _context.Bestelregels.ToListAsync();
        }

        public async Task<Bestelregel?> GetByIdAsync(int id)
        {
            return await _context.Bestelregels.FindAsync(id);
        }

        public async Task AddAsync(Bestelregel bestelregel)
        {
            _context.Bestelregels.Add(bestelregel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bestelregel bestelregel)
        {
            _context.Bestelregels.Update(bestelregel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bestelregel = await _context.Bestelregels.FindAsync(id);
            if (bestelregel != null)
            {
                _context.Bestelregels.Remove(bestelregel);
                await _context.SaveChangesAsync();
            }
        }
    }
}
