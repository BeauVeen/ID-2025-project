using MatrixMobileApp.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public async Task PatchContainerStatusAsync(int containerId, string newStatus, int? userId = null) // userId moet optioneel zijn
        {
            var payload = new
            {
                status = newStatus,
                userId = userId  
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // negeer null values
            });


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/Container/{containerId}")
            {
                Content = content
            };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"PATCH mislukt ({(int)response.StatusCode} {response.ReasonPhrase})\n" +
                    $"Verzonden payload: {json}\n" +
                    $"Antwoord van server: {responseContent}"
                );

                //sponse.EnsureSuccessStatusCode();
            }
        }
    }
}
