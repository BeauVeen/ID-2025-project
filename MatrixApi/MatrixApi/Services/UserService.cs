using MatrixApi.Data;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MatrixApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new NotFoundException($"User with id {id} not found.");
                }
                return user;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error getting user by id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                var existing = await _context.Users.FindAsync(user.UserId);
                if (existing == null)
                {
                    throw new NotFoundException($"User with id {user.UserId} not found.");
                }

                existing.RoleId = user.RoleId;
                existing.Name = user.Name;
                existing.Address = user.Address;
                existing.Zipcode = user.Zipcode;
                existing.City = user.City;
                existing.PhoneNumber = user.PhoneNumber;
                existing.Email = user.Email;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    existing.Password = user.Password;
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error updating user {user.UserId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new NotFoundException($"User with id {id} not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                Console.WriteLine($"Error deleting user {id}: {ex.Message}");
                throw;
            }
        }
    }
}
