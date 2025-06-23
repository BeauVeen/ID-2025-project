using System.Net.Http;
using System.Threading.Tasks;
using MatrixWebApp.Models;
using MatrixWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MatrixWebApp.Pages.Admin.AdminDashboardModel;

namespace MatrixWebApp.Pages
{
    [Authorize(Roles = "Klant")]
    public class CheckoutModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ShoppingCartService _cartService;

        public CheckoutModel(IHttpClientFactory httpClientFactory, ShoppingCartService cartService)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _cartService = cartService;
        }

        public UserDto UserData { get; set; }

        public ShoppingCart Cart => _cartService.Cart;

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            UserData = await _httpClient.GetFromJsonAsync<UserDto>($"api/User/{userId}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            UserData = await _httpClient.GetFromJsonAsync<UserDto>($"api/User/{userId}");

            if (Cart == null || !Cart.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Je winkelwagen is leeg.");
                return Page();
            }

            var orderDto = new OrderDto
            {
                UserId = userId,
                Status = "Pending",
                Signature = null,
                Orderlines = Cart.Items.Select(item => new OrderlineDto
                {
                    ProductId = item.ProductId,
                    Amount = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            // POST naar API
            var response = await _httpClient.PostAsJsonAsync("api/Order", orderDto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Bestelling plaatsen is mislukt.");
                return Page();
            }

            // Winkelwagen leegmaken na succesvolle bestelling
            _cartService.ClearCart();

            TempData["SuccessMessage"] = "Bestelling geplaatst!";

            return RedirectToPage("/OrderConfirmation");
        }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }

    public class OrderDto
    {
        public int UserId { get; set; }
        public string Status { get; set; }
        public string? Signature { get; set; }
        public List<OrderlineDto> Orderlines { get; set; }
    }

    public class OrderlineDto
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
