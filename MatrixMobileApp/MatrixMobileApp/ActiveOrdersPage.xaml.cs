using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.API.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MatrixMobileApp
{
    public partial class ActiveOrdersPage : ContentPage
    {
        private readonly OrderService _orderService;

        public ActiveOrdersPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _orderService = new OrderService(api.Client);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var orders = await _orderService.GetOrdersAsync();
                var activeOrders = orders.Where(o => o.Status == "Active").ToList();
                ActiveOrdersList.ItemsSource = activeOrders;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan actieve bestellingen niet laden: {ex.Message}", "OK");
            }
        }

        private void ActiveOrdersList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Optional: handle order selection
        }

        private async void OnViewOrderClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Order order)
            {
                await Navigation.PushAsync(new OrderDetailPage(order));
            }
        }
    }
}