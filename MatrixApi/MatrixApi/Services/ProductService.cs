using MatrixApi.Data;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Producten.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Producten.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Producten.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Producten.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Producten.FindAsync(id);
            if (product != null)
            {
                _context.Producten.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
