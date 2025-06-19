using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.API.Models
{
    public class Orderline
    {
        public int OrderlineId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int ProductId { get; set; }
    }
}
