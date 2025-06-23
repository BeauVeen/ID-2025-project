using System.Text.Json.Serialization;

namespace MatrixApi.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;
        public ICollection<Orderline> Orderlines { get; set; } = new List<Orderline>();
    }
}
