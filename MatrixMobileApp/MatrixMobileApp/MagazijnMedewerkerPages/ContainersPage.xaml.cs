using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MatrixMobileApp.MagazijnMedewerkerPages
{
    public partial class ContainersPage : ContentPage
    {
        private readonly ContainerService _containerService;

        public ContainersPage()
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
                    .Where(c => c.Status == "In behandeling")
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

        private async void OnContainerTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is int containerId)
            {
                await Navigation.PushAsync(new ContainerOrdersPage(containerId));
            }
        }
    }
}