namespace MatrixApi.Models
{
    public class ContainerOrder
    {
        public int ContainerOrderId { get; set; }

        public int ContainerId { get; set; }
        public Container Container { get; set; } = null!;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
