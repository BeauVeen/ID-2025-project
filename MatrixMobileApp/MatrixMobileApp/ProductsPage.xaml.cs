using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using MatrixMobileApp.API.Models;
using Microsoft.Maui.Controls;

namespace MatrixMobileApp
{
    public partial class ProductsPage : ContentPage
    {
        private readonly ProductService _productService;

        public ProductsPage()
        {
            InitializeComponent();
            var api = new ApiService();
            _productService = new ProductService(api.Client);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var producten = await _productService.GetProductsAsync();
                ProductList.ItemsSource = producten;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan producten niet laden: {ex.Message}", "OK");
            }
        }
    }
}
