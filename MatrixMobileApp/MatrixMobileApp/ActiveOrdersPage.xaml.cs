using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatrixMobileApp
{
    public partial class ActiveOrdersPage : ContentPage
    {
        private readonly ContainerService _containerService;
        private readonly ProductService _productService;
        public ObservableCollection<Container> Containers { get; } = new ObservableCollection<Container>();

        public ActiveOrdersPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _containerService = new ContainerService(api.Client);
            _productService = new ProductService(api.Client);

            BindingContext = this; 

            // Start async initialisatie bij aanmaken van pagina, zodat de data al klaar staat tegen de tijd dat de pagina wordt betreden door user
            _ = LoadContainersAsync(); // '_' zorgt ervoor dat de taak wel uitgevoerd wordt, maar dat er niet wordt gewacht op het resultaat. Dit is overigens ook de enige manier om async methodes op te roepen in een constructor. 
            
        }

        protected override async void OnAppearing() // OnAppearing is veranderd naar async zodat await gebruikt kan worden. 
        {
            base.OnAppearing();

            // Vernieuw de data wanneer de pagina opnieuw verschijnt
            try
            {
                await LoadContainersAsync(); // await wordt nu gebruikt om te garanderen dat data altijd correct geladen is wanneer gebruiker de pagina betreedt..
            }  // await zorgt ervoor dat rest van de code in dezelfde methode (onAppearing in dit geval) pas wordt uitgevoerd nadat de taak is voltooid, maar in dit geval maakt dat geen verschil, omdat er sowieso geen andere methodes uitgevoerd worden na LoadContainersAsync
            catch (Exception ex) 
            {
                await DisplayAlert("Fout", $"Kon orders niet laden: {ex.Message}", "OK");
            }

            
        }

        private async Task LoadContainersAsync()
        {

            int userId = Preferences.Get("user_id", 0);
            if (userId == 0)
            {
                await DisplayAlert("Fout", "Gebruiker niet ingelogd", "OK");
                return;
            }

            var containers = await _containerService.GetContainersByUserIdAsync(userId);

            // Haal productnamen op
            await InitializeProductNamesAsync(containers); 

            Containers.Clear();
            foreach (var container in containers)
            {
                Containers.Add(container);
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