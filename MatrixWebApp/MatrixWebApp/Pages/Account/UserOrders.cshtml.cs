using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MatrixWebApp.Models;

namespace MatrixWebApp.Pages.Account
{
    [Authorize(Roles = "Klant")]
    public class UserOrdersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOrdersModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public UserDto CurrentUserWithOrders { get; set; }

        private HttpClient GetAuthorizedHttpClient()
        {
            var client = _httpClientFactory.CreateClient("MatrixApi");

            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task OnGetAsync()
        {
            var client = GetAuthorizedHttpClient();

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                // Niet ingelogd of geen geldige id
                CurrentUserWithOrders = null;
                return;
            }

            // Oproepen API endpoint om user mét orders op te halen
            CurrentUserWithOrders = await client.GetFromJsonAsync<UserDto>($"api/User/{userId}?includeOrders=true");
            // In jouw backend moet je zorgen dat deze querystring verwerkt wordt om ook orders en orderlines mee te geven
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public List<OrderDto>? Orders { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
            public List<OrderlineDto>? Orderlines { get; set; }
        }

        public class OrderlineDto
        {
            public int OrderlineId { get; set; }
            public string ProductName { get; set; } = null!;
            public int Amount { get; set; }
            public decimal Price { get; set; }
        }
    }
}
