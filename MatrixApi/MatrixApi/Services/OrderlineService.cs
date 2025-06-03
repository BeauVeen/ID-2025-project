using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class OrderlineService
    {
        private readonly AppDbContext _context;

        public OrderlineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Orderline>> GetAllAsync()
        {
            try
            {
                return await _context.Orderlines.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all orderlines: {ex.Message}");
                throw;
            }
        }

        public async Task<Orderline> GetByIdAsync(int id)
        {
            try
            {
                var orderline = await _context.Orderlines.FindAsync(id);
                if (orderline == null)
                {
                    throw new NotFoundException($"Orderline with id {id} not found.");
                }
                return orderline;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting orderline by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Orderline> AddAsync(Orderline orderline)
        {
            try
            {
                _context.Orderlines.Add(orderline);
                await _context.SaveChangesAsync();
                return orderline;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding orderline: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Orderline orderline)
        {
            try
            {
                var existing = await _context.Orderlines.FindAsync(orderline.OrderlineId);
                if (existing == null)
                {
                    throw new NotFoundException($"Orderline with id {orderline.OrderlineId} not found.");
                }

                existing.OrderId = orderline.OrderId;
                existing.ProductId = orderline.ProductId;
                existing.Amount = orderline.Amount;
                existing.Price = orderline.Price;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating orderline {orderline.OrderlineId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var orderline = await _context.Orderlines.FindAsync(id);
                if (orderline == null)
                {
                    throw new NotFoundException($"Orderline with id {id} not found.");
                }

                _context.Orderlines.Remove(orderline);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting orderline {id}: {ex.Message}");
                throw;
            }
        }
    }
}
