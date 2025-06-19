using System.Globalization;
using MatrixMobileApp.API.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MatrixMobileApp
{
    public partial class ContainerPage : ContentPage
    {
        public Container Container { get; }

        public ContainerPage(Container container)
        {
            InitializeComponent();
            Container = container;
            BindingContext = this;

            Title = $"Container {container.ContainerId}";

            // Voeg converters toe
            Resources.Add("CountToHeightConverter", new CountToHeightConverter());
            Resources.Add("OrderTotalConverter", new OrderTotalConverter());
        }

    }

    // Converter voor aantal orderlines naar hoogte
    public class CountToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count * 30; // 30 pixels per item
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter voor totaalbedrag order
    public class OrderTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Orderline> orderlines)
            {
                var total = orderlines.Sum(ol => ol.Price * ol.Amount);
                return $"Totaal: €{total:N2}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}