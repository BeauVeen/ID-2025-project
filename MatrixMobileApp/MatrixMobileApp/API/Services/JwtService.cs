using MatrixMobileApp.API.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MatrixMobileApp.API.Services
{
    public class JwtService
    {
        private readonly HttpClient _client;

        public JwtService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AuthResponse> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("/api/User/authenticate", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Controleer op specifieke HTTP statuscodes
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("Verkeerd e-mailadres of wachtwoord");
                    }

                    throw new Exception($"Fout bij inloggen: {response.StatusCode}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, options);

                if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
                {
                    throw new Exception("Inloggen mislukt, geen token ontvangen");
                }

                return authResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AuthenticateAsync: {ex}");
                throw; // Gooi de exception opnieuw zodat de LoginPage deze kan afhandelen
            }
        }
    }
}