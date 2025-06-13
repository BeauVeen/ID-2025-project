using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.ViewModels;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatrixMobileApp
{
    public partial class OrderDetailPage : ContentPage
    {
        private readonly Order _order;

        public OrderDetailPage(Order order)
        {
            InitializeComponent();
            _order = order;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Fetch all orderlines
            var api = new API.ApiService();
            var orderlineService = new OrderlineService(api.Client);

            var allOrderlines = await orderlineService.GetOrderlinesAsync();

            var productDetails = allOrderlines
                .Select(ol => new OrderProductViewModel
                {
                    Name = ol.ProductName,
                    Amount = ol.Amount,
                    Price = ol.Price
                })
                .ToList();

            ProductsList.ItemsSource = productDetails;
        }
    }
}