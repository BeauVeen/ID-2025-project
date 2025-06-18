using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.ViewModels
{
    public class ProductChecklistItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public bool IsPicked { get; set; }
    }
}
