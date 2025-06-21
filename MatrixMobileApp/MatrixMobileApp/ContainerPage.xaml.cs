using System.Globalization;
using MatrixMobileApp.API.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.API;

namespace MatrixMobileApp
{
    public partial class ContainerPage : ContentPage
    {
        public Container Container { get; }
        private readonly ProductService productService;

        public ContainerPage(Container container)
        {
            InitializeComponent();
            var api = new ApiService();
            productService = new ProductService(api.Client);

            Container = container;
            BindingContext = this;

            Title = $"Container {container.ContainerId}";
            Resources.Add("CountToHeightConverter", new CountToHeightConverter());

            // Start async initialisatie
            _ = InitializeOrderlineProductNamesAsync();
        }

        private async Task InitializeOrderlineProductNamesAsync()
        {
            // Haal alle unieke productIds op uit alle orderlines
            var productIds = Container.ContainerOrders
                .SelectMany(co => co.Order.Orderlines)
                .Select(ol => ol.ProductId)
                .Distinct()
                .ToList();

            // Haal alle producten op en maak een dictionary
            var productDict = new Dictionary<int, string>();
            foreach (var productId in productIds)
            {
                var product = await productService.GetProductByIdAsync(productId);
                if (product != null)
                    productDict[productId] = product.Name;
            }

            // Zet de productnaam op elke orderline
            foreach (var containerOrder in Container.ContainerOrders)
            {
                foreach (var orderline in containerOrder.Order.Orderlines)
                {
                    if (productDict.TryGetValue(orderline.ProductId, out var name))
                        orderline.ProductName = name;
                    else
                        orderline.ProductName = "Onbekend";
                }
            }

            // Forceer UI update
            OnPropertyChanged(nameof(Container));

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
}