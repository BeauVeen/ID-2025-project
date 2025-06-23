using MatrixApi.Data;
using MatrixApi.DTOs;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class ContainerService
    {
        private readonly AppDbContext _context;

        public ContainerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Container>> GetAllAsync()
        {
            try
            {
                return await _context.Containers
                    .Include(c => c.User)
                    .Include(c => c.ContainerOrders)
                        .ThenInclude(co => co.Order)
                            .ThenInclude(o => o.Orderlines)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all containers: {ex.Message}");
                throw;
            }
        }

        public async Task<Container> GetByIdAsync(int id)
        {
            try
            {
                var container = await _context.Containers
                    .Include(c => c.User)
                    .Include(c => c.ContainerOrders)
                        .ThenInclude(co => co.Order)
                            .ThenInclude(o => o.Orderlines)
                    .FirstOrDefaultAsync(c => c.ContainerId == id);

                if (container == null)
                    throw new NotFoundException($"Container with id {id} not found.");

                return container;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting container by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Container> AddAsync(ContainerCreateDto dto)
        {
            try
            {
                var container = new Container
                {
                    CreatedAt = DateTime.UtcNow,
                    Status = dto.Status,
                    UserId = dto.UserId,
                    ContainerOrders = dto.OrderIds.Select(orderId => new ContainerOrder
                    {
                        OrderId = orderId
                    }).ToList()
                };

                _context.Containers.Add(container);
                await _context.SaveChangesAsync();

                return container;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding container: {ex.Message}");
                throw;
            }
        }

        public async Task<Container> UpdateStatusAsync(int containerId, string newStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newStatus))
                    throw new ArgumentException("Status cannot be null or empty.", nameof(newStatus));

                var container = await _context.Containers
                    .Include(c => c.ContainerOrders)
                        .ThenInclude(co => co.Order)
                    .FirstOrDefaultAsync(c => c.ContainerId == containerId);

                if (container == null)
                    throw new NotFoundException($"Container with id {containerId} not found.");

                container.Status = newStatus;

                foreach (var containerOrder in container.ContainerOrders)
                {
                    if (containerOrder.Order != null)
                    {
                        containerOrder.Order.Status = newStatus;
                    }
                }

                await _context.SaveChangesAsync();

                return container;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating container and related order statuses: {ex}");
                throw;
            }
        }



        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var container = await _context.Containers.FindAsync(id);
                if (container == null)
                    throw new NotFoundException($"Container with id {id} not found.");

                _context.Containers.Remove(container);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting container {id}: {ex.Message}");
                throw;
            }
        }
    }
}
