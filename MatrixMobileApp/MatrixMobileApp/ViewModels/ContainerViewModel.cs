using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.ViewModels
{
    public class ContainerViewModel
    {
        public int ContainerId { get; set; }
        public int OrderCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
