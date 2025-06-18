namespace MatrixApi.DTOs
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public string Status { get; set; }
        public IFormFile? Signature { get; set; } 
        public List<OrderlineCreateDto> Orderlines { get; set; }
    }
}
