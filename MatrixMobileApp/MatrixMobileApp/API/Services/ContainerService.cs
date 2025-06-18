using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class ContainerService
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ContainerService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Container>> GetContainersAsync()
        {
            var response = await _client.GetAsync("/api/Container");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Container>>(json, _jsonOptions) ?? new List<Container>();
        }

        public async Task UpdateContainerAsync(Container container)
        {
            var json = JsonSerializer.Serialize(container);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/Container/{container.ContainerId}", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
