using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MatrixWebApp.Models;
using MatrixWebApp.Extensions;
using MatrixWebApp.ViewComponents;
using System.Linq;

namespace MatrixWebApp.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsModel> _logger;

        public List<ProductDto> Products { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        public Dictionary<int, string> CategoryNamesById { get; set; } = new();

        // Nieuwe property voor hoogste prijs
        public decimal MaxPrice { get; set; } = 0m;

        public ProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
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

            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();

            cart.AddItem(new CartItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Picture = product.Picture,
                Quantity = quantity
            });

            HttpContext.Session.Set("Cart", cart);
            HttpContext.Session.SetInt32("CartItemCount", cart.TotalItems);

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

                MaxPrice = Products.Max(p => p.Price);
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