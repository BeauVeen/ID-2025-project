using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;

namespace MatrixMobileApp.MagazijnMedewerkerPages
{
    public partial class ContainerOrdersPage : ContentPage
    {
        private readonly int _containerId;
        private readonly ContainerService _containerService;


        public ContainerOrdersPage(int containerId)
        {
            InitializeComponent();
            _containerId = containerId;

            var api = new ApiService();
            _containerService = new ContainerService(api.Client);


        }
  
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var containers = await _containerService.GetContainersAsync();
                var container = containers.FirstOrDefault(c => c.ContainerId == _containerId);

                if (container != null)
                {
                    HeaderLabel.Text = $"Container {container.ContainerId} - Orders";

                    var orderViewModels = container.ContainerOrders
                        .Select(co => new OrderWithReadyViewModel
                        {
                            OrderId = co.Order.OrderId,
                            Status = co.Order.Status,
                            IsReady = Preferences.Get($"order_{co.Order.OrderId}_ready", false),
                            ProductsList = co.Order.Orderlines.Select(ol => new OrderProductViewModel
                            {
                                ProductName = ol.ProductName,
                                Amount = ol.Amount,
                                Price = ol.Price
                            }).ToList()
                        })
                        .ToList();

                    OrdersList.ItemsSource = orderViewModels;
                    UpdateAcceptButtonVisibility(orderViewModels);
                }
                else
                {
                    await DisplayAlert("Fout", "Container niet gevonden.", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan containergegevens niet laden: {ex.Message}", "OK");
            }
        }

        private async void OnOrderTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int orderId)
            {
                await Navigation.PushAsync(new OrderProductsPage(orderId));
            }
        }

        private void UpdateAcceptButtonVisibility(List<OrderWithReadyViewModel> orders)
        {
            AcceptButton.IsVisible = !orders.Any()
                || orders.All(o => o.ProductsList == null || o.ProductsList.Count == 0)
                || (orders.Any() && orders.All(o => o.IsReady));
        }

        private async void OnAcceptClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Bevestigen",
                "Weet je zeker dat je de status van deze container wilt zetten op 'Klaar voor Verzending'?",
                "Ja", "Nee");

            if (confirm)
            {
                try
                {
                    await _containerService.PatchContainerStatusAsync(_containerId, "Klaar voor verzending");
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                    await DisplayAlert("Succes", "Containerstatus is bijgewerkt.", "OK");
                    
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fout", $"Status bijwerken mislukt: {ex.Message}", "OK");
                }
            }
        }
    }
}
