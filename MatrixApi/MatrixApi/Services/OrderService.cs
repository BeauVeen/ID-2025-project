using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all orders: {ex.Message}");
                throw;
            }
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    throw new NotFoundException($"Order with id {id} not found.");
                }
                return order;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting order by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Order> AddAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding order: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            try
            {
                var existing = await _context.Orders.FindAsync(order.OrderId);
                if (existing == null)
                {
                    throw new NotFoundException($"Order with id {order.OrderId} not found.");
                }

                existing.UserId = order.UserId;
                existing.Status = order.Status;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating order {order.OrderId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    throw new NotFoundException($"Order with id {id} not found.");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting order {id}: {ex.Message}");
                throw;
            }
        }
    }
}
