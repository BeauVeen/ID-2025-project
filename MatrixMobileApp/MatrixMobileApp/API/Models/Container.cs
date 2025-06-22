using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MatrixMobileApp.API.Models
{
    public class Container 
    {
        public int ContainerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ContainerOrder> ContainerOrders { get; set; } = new();
    }

    public class ContainerOrder : INotifyPropertyChanged //  INotifyPropertyChanged wordt toegevoegd om de dropdown iconen dynamisch aan te passen bij expand/unexpand 
    {
        public int ContainerOrderId { get; set; }
        public int ContainerId { get; set; }
        public string Container { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded; // waarde van IsExpanded wordt omgedraaid 
        }



        private bool _isExpanded; // private bool om de huidige waarde van de dropdown op te slaan 
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded)); // signaleert aan de UI dat IsExpanded is gewijzigd (bool is veranderd)
                    OnPropertyChanged(nameof(VisibleOrderlines)); // signaleert dat de zichtbare orderlines zijn gewijzigd
                    OnPropertyChanged(nameof(DropdownIcon)); // signaleert aan de UI dat het dropdown icoon moet worden aangepast
                }
            }
        }

        public IEnumerable<Orderline> VisibleOrderlines => IsExpanded ? Order.Orderlines : Enumerable.Empty<Orderline>(); // geeft de zichtbare orderlines terug, afhankelijk van de waarde van IsExpanded. Als IsExpanded true is, worden alle orderlines getoond

        public string DropdownIcon => IsExpanded ? "dropdownup.png" : "dropdowndown.png";  // dropdown iconen worden aangepast op basis van de waarde van IsExpanded

    
        public event PropertyChangedEventHandler? PropertyChanged;  // hoort bij INotifyPropertyChanged. hierdoor weet de UI dat er iets veranderd is
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
