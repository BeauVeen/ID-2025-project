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

        private async void OnDeliverClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Bevestigen",
                "Weet je zeker dat je de status van deze container wilt zetten op 'Onderweg'?",
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    // haal userId op uit preferences (opgeslagen na inloggen)
                    int userId = Preferences.Get("user_id", 0);

                    if (userId == 0) // Controleer op geldige ID
                    {
                        await DisplayAlert("Fout", "Gebruiker niet ingelogd of ongeldige ID", "OK");
                        return;
                    }

                    var containerService = new ContainerService(new ApiService().Client);

                    await containerService.PatchContainerStatusAsync(
                        Container.ContainerId,
                        "Onderweg",
                        userId // bij bezorgen van een container wordt de bezorger gekoppeld aan de container
                    );

                    await DisplayAlert("Succes", "Containerstatus is bijgewerkt naar 'Onderweg'.", "OK");
                    await Shell.Current.GoToAsync("//RoutePage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout", $"Status bijwerken mislukt: {ex.Message}", "OK");
                }
            }
        }

        private void OnToggleExpandClicked(object sender, EventArgs e)
        {
            if (sender is Image image && image.BindingContext is ContainerOrder containerOrder) // Duidelijk maken dat de sender een Image is en dat de BindingContext een ContainerOrder is
            {
                containerOrder.ToggleExpand();
            }
        }
    }
}