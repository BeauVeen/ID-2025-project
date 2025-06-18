using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MatrixMobileApp
{
    public partial class ContainerOrdersPage : ContentPage
    {
        private readonly int _containerId;

        public ContainerOrdersPage(int containerId)
        {
            InitializeComponent();
            _containerId = containerId;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var api = new ApiService();
            var containerService = new ContainerService(api.Client);

            var containers = await containerService.GetContainersAsync();
            var container = containers.FirstOrDefault(c => c.ContainerId == _containerId);

            if (container != null)
            {
                HeaderLabel.Text = $"Container {container.ContainerId} - Orders";
                OrdersList.ItemsSource = container.ContainerOrders.Select(co => co.Order).ToList();
            }
            else
            {
                await DisplayAlert("Fout", "Container niet gevonden.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}