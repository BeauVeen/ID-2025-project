namespace MatrixApi.Models
{
    public class Orderline
    {
        public int OrderlineId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Amount {  get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
