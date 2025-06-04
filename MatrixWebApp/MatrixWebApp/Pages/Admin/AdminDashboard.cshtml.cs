using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MatrixWebApp.Pages.Admin
{
    public class AdminDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<string> CustomerMonths { get; set; } = new();
        public List<int> CustomerCounts { get; set; } = new();

        public List<string> OrdersMonths { get; set; } = new();
        public List<int> OrdersCounts { get; set; } = new();

        public int OrdersPlaced { get; set; }
        public int CustomersCount { get; set; }

        public AdminDashboardModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public async Task OnGetAsync()
        {
            // Customers by month
            var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/User") ?? new List<UserDto>();
            CustomersCount = users.Count;

            var groupedUsers = users
                .Where(u => u.CreatedAt != null)
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            CustomerMonths = groupedUsers
                .Select(g => $"{g.Key.Year}-{g.Key.Month:00}")
                .ToList();
            CustomerCounts = groupedUsers
                .Select(g => g.Count())
                .ToList();

            // Orders placed by month (no status filter)
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
    }
}