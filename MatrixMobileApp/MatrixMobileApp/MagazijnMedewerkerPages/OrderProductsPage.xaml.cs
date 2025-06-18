using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;

namespace MatrixMobileApp.MagazijnMedewerkerPages
{
    public partial class OrderProductsPage : ContentPage
    {
        private readonly int _orderId;
        private List<ProductChecklistItemViewModel> _productList;

        public OrderProductsPage(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var api = new ApiService();
            var orderService = new OrderService(api.Client);

            var orders = await orderService.GetOrdersAsync();
            var order = orders.FirstOrDefault(o => o.OrderId == _orderId);

            if (order != null)
            {
                HeaderLabel.Text = $"Order {order.OrderId} - Producten";
                _productList = order.Orderlines.Select(ol => new ProductChecklistItemViewModel
                {
                    Name = ol.ProductName,
                    Amount = ol.Amount,
                    Price = ol.Price,
                    // Use OrderlineId for unique key
                    IsPicked = Preferences.Get($"order_{_orderId}_product_{ol.OrderlineId}_picked", false),
                    OrderlineId = ol.OrderlineId
                }).ToList();

                ProductsList.ItemsSource = _productList;
                UpdateReadyLabel();
            }
            else
            {
                await DisplayAlert("Fout", "Order niet gevonden.", "OK");
                await Navigation.PopAsync();
            }
        }

        private void OnProductCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is ProductChecklistItemViewModel product)
            {
                Preferences.Set($"order_{_orderId}_product_{product.OrderlineId}_picked", product.IsPicked);
            }

            // If all products are picked, mark order as ready
            if (_productList.All(p => p.IsPicked))
            {
                Preferences.Set($"order_{_orderId}_ready", true);
            }
            else
            {
                Preferences.Set($"order_{_orderId}_ready", false);
            }

            UpdateReadyLabel();
        }

        private void UpdateReadyLabel()
        {
            ReadyLabel.IsVisible = _productList != null && _productList.All(p => p.IsPicked);
        }

        private async void OnDoneClicked(object sender, EventArgs e)
        {
            if (_productList.All(p => p.IsPicked))
            {
                await DisplayAlert("Succes", "Alle producten zijn gepickt!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Let op", "Niet alle producten zijn gepickt.", "OK");
            }
        }
    }
}