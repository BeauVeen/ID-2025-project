using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.ViewModels
{
    public class OrderWithReadyViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Orderline> Orderlines { get; set; } = new();
        public bool IsReady { get; set; }
    }
}
