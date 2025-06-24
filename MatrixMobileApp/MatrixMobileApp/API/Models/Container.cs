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
        public int? UserId { get; set; } = null; // wanneer een bezorger de orders van een container gaat bezorgen, wordt de userid van de bezorger gekoppeld aan de container zodat hij de orders op de Actieve Orders pagina kan zien
        public List<ContainerOrder> ContainerOrders { get; set; } = new();
        public int OrderCount
        {
            get
            {
                return ContainerOrders.Count; // geeft het totale aantal orders in de container terug
            }
        } 
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

        private bool _isExpanded;  // private bool om de huidige waarde van de dropdown op te slaan 
        public bool IsExpanded
        {
            get
            {
                return _isExpanded; // haal de huidige waarde van _isExpanded op
            }
            set // wordt uitgevoerd wanneer de waarde van IsExpanded wordt aangepast, oftewel wanneer de gebruiker op de dropdown klikt
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));  // signaleert aan de UI dat IsExpanded is gewijzigd (bool is veranderd)
                    OnPropertyChanged(nameof(VisibleOrderlines));  // signaleert dat de zichtbare orderlines zijn gewijzigd
                    OnPropertyChanged(nameof(DropdownIcon)); // signaleert aan de UI dat het dropdown icoon moet worden aangepast
                }
            }
        }


        public IEnumerable<Orderline> VisibleOrderlines // geeft de zichtbare orderlines terug, afhankelijk van de waarde van IsExpanded. Als IsExpanded true is, worden alle orderlines getoond
        {
            get
            {
                IEnumerable<Orderline> result;

                if (IsExpanded == true)
                {
                    result = Order.Orderlines; // Geef alle orderlines terug als IsExpanded true is
                }
                else
                {
                    result = Enumerable.Empty<Orderline>(); // Geef lege lijst terug als IsExpanded false is
                }

                return result;
            }
        }

        public string DropdownIcon // dropdown iconen worden aangepast op basis van de waarde van IsExpanded
        {
            get
            {
                if (IsExpanded)
                {
                    return "dropdownup.png";
                }
                else
                {
                    return "dropdowndown.png";
                }
            }
        }  


        public event PropertyChangedEventHandler? PropertyChanged;  // hoort bij INotifyPropertyChanged. hierdoor weet de UI dat er iets veranderd is
                                                                   
        protected void OnPropertyChanged(string propertyName) // Methode om het PropertyChanged-event te triggeren.
        {
         
            if (PropertyChanged != null)
            {             
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName)); // 'this' = huidig object, PropertyChangedEventArgs(propertyName) = info over de gewijzigde property

            }
        }

    }
}
