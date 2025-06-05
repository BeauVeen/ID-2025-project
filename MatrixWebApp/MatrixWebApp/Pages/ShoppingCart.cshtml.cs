using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MatrixWebApp.Models;
using MatrixWebApp.Extensions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MatrixWebApp.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ShoppingCartModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public ShoppingCart Cart { get; set; }

        public void OnGet()
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
        }

        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            var item = Cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                item.Quantity = quantity;
                HttpContext.Session.Set("Cart", Cart);
                HttpContext.Session.SetInt32("CartItemCount", Cart.TotalItems);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int productId)
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            var item = Cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                Cart.Items.Remove(item);
                HttpContext.Session.Set("Cart", Cart);
                HttpContext.Session.SetInt32("CartItemCount", Cart.TotalItems);

                if (item.Quantity > 1)
                {
                    TempData["WarningMessage"] = $"{item.Name} (x{item.Quantity}) is verwijderd uit je winkelwagen.";
                }
                else
                {
                    TempData["WarningMessage"] = $"{item.Name} is verwijderd uit je winkelwagen.";
                }
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Je moet ingelogd zijn om een bestelling te plaatsen.";
                return RedirectToPage();
            }

            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (!Cart.Items.Any())
            {
                TempData["WarningMessage"] = "Je winkelwagen is leeg.";
                return RedirectToPage();
            }

            var order = new
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                Orderlines = Cart.Items.Select(i => new
                {
                    ProductId = i.ProductId,
                    Amount = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("api/Order", order);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("CartItemCount");

                var createdOrder = await response.Content.ReadFromJsonAsync<OrderResponse>();
                TempData["Message"] = $"Bestelling #{createdOrder.OrderId} is geplaatst!";
                return RedirectToPage("/OrderConfirmation", new { orderId = createdOrder.OrderId });
            }
            else
            {
                TempData["ErrorMessage"] = "Er ging iets mis bij het plaatsen van je bestelling.";
                return RedirectToPage();
            }
        }

        private class OrderResponse
        {
            public int OrderId { get; set; }
        }
    }
}