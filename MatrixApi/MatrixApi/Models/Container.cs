namespace MatrixApi.Models
{
    public class Container
    {
        public int ContainerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "In behandeling";
        public ICollection<ContainerOrder> ContainerOrders { get; set; } = new List<ContainerOrder>();
    }
}
