namespace MatrixApi.DTOs
{
    public class OrderlineDto
    {
        public int OrderlineId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public ProductNoPic Product { get; set; } = null!;
    }
}
