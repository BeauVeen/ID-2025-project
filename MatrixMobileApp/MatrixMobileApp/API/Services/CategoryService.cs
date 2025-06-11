using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class CategoryService
    {
        private readonly HttpClient _client;

        public CategoryService(HttpClient client)
        { 
            _client = client;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _client.GetAsync("/api/Category");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true}) ?? new List<Category>();
        }
    }
}
