using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class RoleService
    {
        private readonly HttpClient _client;

        public RoleService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            var response = await _client.GetAsync("/api/Role");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Role>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Role>();
        }
    }
}
