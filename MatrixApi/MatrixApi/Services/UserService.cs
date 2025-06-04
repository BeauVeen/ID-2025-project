using MatrixApi.Data;
using MatrixApi.DTOs;
using MatrixApi.Exceptions;
using MatrixApi.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            try
            {
                var existing = await _context.Users.FindAsync(id);

                if (existing == null)
                {
                    throw new NotFoundException($"User with id {id} not found.");
                }

                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email && u.UserId != id);

                if (emailExists)
                {
                    throw new EmailAlreadyInUseException("Email already in use");
                }

                existing.RoleId = dto.RoleId;
                existing.Name = dto.Name;
                existing.Address = dto.Address;
                existing.Zipcode = dto.Zipcode;
                existing.City = dto.City;
                existing.PhoneNumber = dto.PhoneNumber;
                existing.Email = dto.Email;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (EmailAlreadyInUseException) 
            {
                throw;
            }
            catch (NotFoundException) 
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user {id}: {ex.Message}");
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
