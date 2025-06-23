using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMobileApp.API.Models
{
    public class Orderline : INotifyPropertyChanged
    {
        public int OrderlineId { get; set; }
        public int OrderId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }

        private string? _productName;
        public string? ProductName // INotifyPropertyChanged is nodig om ProductNaam te updaten en correct te laten zien op de Containers pagina, omdat de asynchrone functie van ProductNaam ophalen later wordt uitgevoerd dan de UI van de pagina.
        {
            get => _productName;
            set
            {
                // alleen als de waarde echt verandert, voeren we de update uit.
                if (_productName != value)
                {
                    _productName = value;
                    OnPropertyChanged(nameof(ProductName));  // signaleert aan de UI dat ProductName is gewijzigd, zodat data binding werkt.
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


