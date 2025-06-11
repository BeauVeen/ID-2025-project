using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class OrderlineService
    {
        private readonly HttpClient _client;

        public OrderlineService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Orderline>> GetOrderlinesAsync()
        {
            var response = await _client.GetAsync("/api/Orderline");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Orderline>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Orderline>();
        }
    }
}
