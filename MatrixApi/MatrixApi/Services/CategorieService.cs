using MatrixApi.Data;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class CategorieService
    {
        private readonly AppDbContext _context;

        public CategorieService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Categorie>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Categorie?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Categorie categorie)
        {
            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categorie categorie)
        {
            _context.Categories.Update(categorie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie != null)
            {
                _context.Categories.Remove(categorie);
                await _context.SaveChangesAsync();
            }
        }
    }
}
