using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using MatrixMobileApp.MagazijnMedewerkerPages;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MatrixMobileApp
{
    public partial class DeliveryContainersPage : ContentPage
    {
        private readonly ContainerService _containerService;

        public DeliveryContainersPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _containerService = new ContainerService(api.Client);
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
            if (e.CurrentSelection.FirstOrDefault() is ContainerViewModel selectedContainer)
            {
                await Navigation.PushAsync(new ContainerOrdersPage(selectedContainer.ContainerId));
            }

            // Deselect item after navigation for better UX
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}