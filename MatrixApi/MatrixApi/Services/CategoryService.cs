using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {  
            _context = context; 
        }

        public async Task<List<Category>> GetAllAsync()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all categories: {ex.Message}");
                throw;
            }
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    throw new NotFoundException($"Category with id {id} not found.");
                }
                return category;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting category by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Category> AddAsync(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                var existing = await _context.Categories.FindAsync(category.CategoryId);
                if (existing == null)
                { 
                    throw new NotFoundException($"Category with id {category.CategoryId} not found.");
                }

                existing.CategoryName = category.CategoryName;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!( ex is NotFoundException))
            {
                Console.WriteLine($"Error updating category {category.CategoryId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    throw new NotFoundException($"Category with id {id} not found.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting category {id}: {ex.Message}");
                throw;
            }
        }
    }
}
