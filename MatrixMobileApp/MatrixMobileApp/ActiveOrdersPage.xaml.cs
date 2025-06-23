using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using System.Collections.ObjectModel;

namespace MatrixMobileApp
{
    public partial class ActiveOrdersPage : ContentPage
    {
        private readonly ContainerService _containerService;
        private readonly ProductService _productService;
        public ObservableCollection<Container> Containers { get; } = new();

        public ActiveOrdersPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _containerService = new ContainerService(api.Client);
            _productService = new ProductService(api.Client);

            BindingContext = this;

            // Start async initialisatie
            _ = LoadContainersAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Vernieuw de data wanneer de pagina opnieuw verschijnt
            _ = LoadContainersAsync();
        }

        private async Task LoadContainersAsync()
        {
            try
            {
                int userId = Preferences.Get("user_id", 0);
                if (userId == 0)
                {
                    await DisplayAlert("Fout", "Gebruiker niet ingelogd", "OK");
                    return;
                }

                var containers = await _containerService.GetContainersByUserIdAsync(userId);

                // Vul productnamen in
                await InitializeProductNamesAsync(containers);

                Containers.Clear();
                foreach (var container in containers)
                {
                    Containers.Add(container);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kon orders niet laden: {ex.Message}", "OK");
            }
        }

        private async Task InitializeProductNamesAsync(List<Container> containers)
        {
            var productIds = containers
                .SelectMany(c => c.ContainerOrders)
                .SelectMany(co => co.Order.Orderlines)
                .Select(ol => ol.ProductId)
                .Distinct()
                .ToList();

            var productDict = new Dictionary<int, string>();
            foreach (var productId in productIds)
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product != null)
                    productDict[productId] = product.Name;
            }

            foreach (var container in containers)
            {
                foreach (var containerOrder in container.ContainerOrders)
                {
                    foreach (var orderline in containerOrder.Order.Orderlines)
                    {
                        if (productDict.TryGetValue(orderline.ProductId, out var name))
                            orderline.ProductName = name;
                        else
                            orderline.ProductName = "Onbekend";
                    }
                }
            }
        }

        private void OnToggleExpandClicked(object sender, EventArgs e)
        {
            if (sender is Image image && image.BindingContext is ContainerOrder containerOrder)
            {
                containerOrder.ToggleExpand();
            }
        }

    }
}