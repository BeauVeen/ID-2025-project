using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MatrixMobileApp.API.Models;

namespace MatrixMobileApp.API.Services
{
    internal class ManualContainerCodeService
    {
        private readonly HttpClient _client;

        public ManualContainerCodeService(HttpClient client)
        {
            _client = client;
        }

        public async Task<Container> GetContainerById(int containerId)
        {
            var response = await _client.GetAsync($"/api/Container/{containerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                
                return null; // null wordt gebruikt om specifieke errormessage te tonen bij verkeerde invoer containerId
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Container>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}