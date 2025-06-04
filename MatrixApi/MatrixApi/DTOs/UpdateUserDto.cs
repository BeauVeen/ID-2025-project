namespace MatrixApi.DTOs
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string? Password { get; set; } 
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
    }
}