namespace MatrixApi.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;
        public List<OrderlineDto>? Orderlines { get; set; }
    }
}
