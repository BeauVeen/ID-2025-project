using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class AdminDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<string> CustomerMonths { get; set; } = new();
        public List<int> CustomerCounts { get; set; } = new();

        public List<string> OrdersMonths { get; set; } = new();
        public List<int> OrdersCounts { get; set; } = new();

        public int OrdersPlaced { get; set; }
        public int CustomersCount { get; set; }

        // Nieuw: lijst lage voorraad producten
        public List<ProductDto> LageVoorraadProducten { get; set; } = new();

        public AdminDashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public async Task OnGetAsync()
        {
            //  klanten tellen en groeperen
            var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/User") ?? new List<UserDto>();
            CustomersCount = users.Count;

            var groupedUsers = users
                .Where(u => u.CreatedAt != null)
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            CustomerMonths = groupedUsers.Select(g => $"{g.Key.Year}-{g.Key.Month:00}").ToList();
            CustomerCounts = groupedUsers.Select(g => g.Count()).ToList();

            //orders tellen en groeperen
            var orders = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/Order") ?? new List<OrderDto>();

            var ordersByMonth = orders
                .Where(o => o.CreatedAt != null)
                .GroupBy(o => new { o.CreatedAt.Value.Year, o.CreatedAt.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    Count = g.Count()
                })
                .ToList();

            OrdersMonths = ordersByMonth.Select(x => x.Month).ToList();
            OrdersCounts = ordersByMonth.Select(x => x.Count).ToList();

            OrdersPlaced = orders.Count;

            // Nieuw: producten ophalen en filteren op voorraad <= 10
            var producten = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product") ?? new List<ProductDto>();

            LageVoorraadProducten = producten
                .Where(p => p.Stock <= 10)
                .OrderBy(p => p.Stock)
                .ToList();
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class OrderDto
        {
            public int OrderId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public string Status { get; set; }
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }          
            public string Description { get; set; }
            public decimal Price { get; set; }        
            public int Stock { get; set; }
            public byte[]? Picture { get; set; }
        }
    }
}
