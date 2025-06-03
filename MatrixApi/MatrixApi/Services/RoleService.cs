using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all categories: {ex.Message}");
                throw;
            }
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    throw new NotFoundException($"Role with id {id} not found.");
                }
                return role;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting role by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Role> AddAsync(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding role: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            try
            {
                var existing = await _context.Roles.FindAsync(role.RoleId);
                if (existing == null)
                {
                    throw new NotFoundException($"Role with id {role.RoleId} not found.");
                }

                existing.RoleName = role.RoleName;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating role {role.RoleId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    throw new NotFoundException($"Role with id {id} not found.");
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting role {id}: {ex.Message}");
                throw;
            }
        }
    }
}
