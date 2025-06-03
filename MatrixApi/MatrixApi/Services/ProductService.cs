using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;
using MatrixApi.DTOs;

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
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all products: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    throw new NotFoundException($"Product with id {id} not found.");
                }
                return product;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting product by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> AddAsync(ProductDto dto)
        {
            try
            {
                var product = new Product
                {
                    CategoryId = dto.CategoryId,
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    Picture = dto.Picture
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                var existing = await _context.Products.FindAsync(product.ProductId);
                if (existing == null)
                {
                    throw new NotFoundException($"Product with id {product.ProductId} not found.");
                }

                existing.CategoryId = product.CategoryId;
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Stock = product.Stock;
                existing.Picture = product.Picture;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating product {product.ProductId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    throw new NotFoundException($"Product with id {id} not found.");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting product {id}: {ex.Message}");
                throw;
            }
        }
    }
}
