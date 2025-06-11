using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class OrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var response = await _client.GetAsync("/api/Order");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Order>();
        }
    }
}
