namespace MatrixApi.DTOs
{
    public class OrderlineCreateDto
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
