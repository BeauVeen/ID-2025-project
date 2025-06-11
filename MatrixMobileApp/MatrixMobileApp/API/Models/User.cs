using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.API.Models
{
    internal class User
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
