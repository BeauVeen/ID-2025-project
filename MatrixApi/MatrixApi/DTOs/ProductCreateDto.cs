namespace MatrixApi.DTOs
{
    public class ProductCreateDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
