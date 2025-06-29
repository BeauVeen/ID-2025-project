using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MatrixWebApp.Extensions;
using MatrixWebApp.ViewComponents;
using System.Linq;
using MatrixWebApp.Services;
using MatrixWebApp.Models;

namespace MatrixWebApp.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsModel> _logger;
        private readonly ShoppingCartService _cartService;

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public Dictionary<int, string> CategoryNamesById { get; set; } = new Dictionary<int, string>();

        public decimal MaxPrice { get; set; } = 0.0m;

        public ProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger, ShoppingCartService cartService)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
            _cartService = cartService;
        }

        public async Task<IActionResult> OnPostAddToCart(int productId, int quantity)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/Product/{productId}");

            if (product == null)
            {
                return NotFound();
            }

            var item = new CartItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Picture = product.Picture
            };

            _cartService.AddProducts(item);

            if (quantity > 1)
            {
                TempData["Message"] = $"{product.Name} (x{quantity}) is toegevoegd aan je winkelwagen.";
            }
            else
            {
                TempData["Message"] = $"{product.Name} is toegevoegd aan je winkelwagen.";
            }

            return RedirectToPage();
        }

        public async Task OnGetAsync(int? categoryId)
        {
            // haal categorieën op
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");

            // vul dictionary met categorieën
            if (categories != null)
            {
                CategoryNamesById = new Dictionary<int, string>();
                foreach (var category in categories)
                {
                    CategoryNamesById.Add(category.CategoryId, category.CategoryName);
                }
            }

            // haal producten op
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

            if (products != null)
            {
                // filter producten als er een categorie is geselecteerd
                if (categoryId.HasValue)
                {
                    Products = new List<ProductDto>();
                    foreach (var product in products)
                    {
                        if (product.CategoryId == categoryId.Value)
                        {
                            Products.Add(product);
                        }
                    }
                }
                else
                {
                    Products = products;
                }

                // bereken maximale prijs
                if (Products.Count > 0)
                {
                    MaxPrice = Products[0].Price;
                    foreach (var product in Products)
                    {
                        if (product.Price > MaxPrice)
                        {
                            MaxPrice = product.Price;
                        }
                    }
                }
                else
                {
                    MaxPrice = 0;
                }

                ViewData["MaxPrice"] = MaxPrice;
            }
            else
            {
                _logger.LogWarning("Geen producten gevonden vanuit de API");
            }
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public byte[] Picture { get; set; }
            public string CategoryName { get; set; }
        }
    }
}