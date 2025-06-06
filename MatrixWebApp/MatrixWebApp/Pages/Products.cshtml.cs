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

        public List<ProductDto> Products { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        public Dictionary<int, string> CategoryNamesById { get; set; } = new();

        // Nieuwe property voor hoogste prijs
        public decimal MaxPrice { get; set; } = 0m;

        public ProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger, ShoppingCartService cartService)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
            _cartService = cartService;
        }

        // Haal categorieën 1 keer op
        private async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            return categories ?? new List<CategoryDto>();
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

            TempData["Message"] = quantity > 1
            ? $"{product.Name} (x{quantity}) is toegevoegd aan je winkelwagen."
            : $"{product.Name} is toegevoegd aan je winkelwagen.";

            return RedirectToPage();
        }

        public async Task OnGetAsync(int? categoryId)
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            if (categories != null)
            {
                CategoryNamesById = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);
            }

            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

            if (products != null)
            {
                Products = categoryId.HasValue
                    ? products.Where(p => p.CategoryId == categoryId.Value).ToList()
                    : products;

                // Enkel deze lijn aanpassen voor foutafhandeling:
                MaxPrice = Products.Any() ? Products.Max(p => p.Price) : 0;
                ViewData["MaxPrice"] = MaxPrice;
            }
            else
            {
                _logger.LogWarning("No products received from API.");
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