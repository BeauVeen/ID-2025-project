namespace MatrixApi.DTOs
{
    public class CreateUserDto
    {
        public string Password { get; set; } = null!;
        public int? RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
    }
}
