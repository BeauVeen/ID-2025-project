using System;
using System.Collections.Generic;

namespace MatrixMobileApp.API.Models
{
    public class Container
    {
        public int ContainerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ContainerOrder> ContainerOrders { get; set; } = new();
    }

    public class ContainerOrder
    {
        public int ContainerOrderId { get; set; }
        public int ContainerId { get; set; }
        public string Container { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
