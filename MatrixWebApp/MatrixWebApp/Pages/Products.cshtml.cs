using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MatrixWebApp.Models;
using MatrixWebApp.Extensions;

namespace MatrixWebApp.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsModel> _logger;

        public List<ProductDto> Products { get; set; } = new();


        public ProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAddToCart(int productId, int quantity)
        {
            // Haal productdetails op van de API
            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/Product/{productId}");

            if (product == null)
            {
                return NotFound();
            }

            // Haal huidige winkelwagen op of maak nieuwe
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Voeg item toe
            cart.AddItem(new CartItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Picture = product.Picture,
                Quantity = quantity
            });

            // Sla winkelwagen op in session
            HttpContext.Session.Set("Cart", cart);
            HttpContext.Session.SetInt32("CartItemCount", cart.TotalItems);

            // Terug naar productpagina met succesmelding
            TempData["SuccessMessage"] = $"{product.Name} is toegevoegd aan je winkelwagen!";
            return RedirectToPage();
        }
        public async Task OnGetAsync(int? categoryId)
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

                if (products != null)
                {

                    Products = categoryId.HasValue
                        ? products.Where(p => p.CategoryId == categoryId.Value).ToList()
                        : products;
                }
                else
                {
                    _logger.LogWarning("No products received from API.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error with retrieving products from API.");
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
        }
    }
}