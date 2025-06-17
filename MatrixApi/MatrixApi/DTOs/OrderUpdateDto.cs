namespace MatrixApi.DTOs
{
    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public IFormFile? Signature { get; set; }
    }
}
