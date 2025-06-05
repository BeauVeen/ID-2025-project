using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;

namespace MatrixApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Zipcode {get; set; }
        public string? City {get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Role? Role { get; set; }
    }
}
