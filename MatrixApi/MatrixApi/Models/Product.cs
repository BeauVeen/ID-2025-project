namespace MatrixApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock {  get; set; }
        public byte[]? Picture { get; set; }

        public Category? Category { get; set; }
        public ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();
    }
}
