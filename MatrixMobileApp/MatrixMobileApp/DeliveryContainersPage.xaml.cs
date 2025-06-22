using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MatrixMobileApp
{
    public partial class DeliveryContainersPage : ContentPage
    {
        private readonly ContainerService _containerService;
        private readonly ManualContainerCodeService _manualContainerService;

        public DeliveryContainersPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _containerService = new ContainerService(api.Client);
            _manualContainerService = new ManualContainerCodeService(api.Client);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var containers = await _containerService.GetContainersAsync();
                var containerViewModels = containers
                    .Where(c => c.Status == "Klaar voor verzending")
                    .Select(c => new ContainerViewModel
                    {
                        ContainerId = c.ContainerId,
                        OrderCount = c.ContainerOrders?.Count ?? 0,
                        Status = c.Status
                    }).ToList();

                ContainersList.ItemsSource = containerViewModels;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan containers niet laden: {ex.Message}", "OK");
            }
        }

        private async void OnContainerSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not ContainerViewModel selectedContainer)
            {
                return;
            }

            try
            {
                var container = await _manualContainerService.GetContainerById(selectedContainer.ContainerId);

                if (container == null)
                {
                    await DisplayAlert("Fout", "Geen container met dit containernummer gevonden", "OK");
                    return;
                }

                await Navigation.PushAsync(new ContainerPage(container));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan container niet laden: {ex.Message}", "OK");
            }

            // Deselect item after navigation for better UX
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}