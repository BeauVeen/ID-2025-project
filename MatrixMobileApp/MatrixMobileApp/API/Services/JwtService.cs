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
                Console.WriteLine("Serializing login request");
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine("Sending request to API");
                var response = await _client.PostAsync("/api/User/authenticate", content);
                Console.WriteLine($"Response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                    return null;
                }

                Console.WriteLine("Reading response content");
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw response: {responseJson}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson, options);

                if (authResponse == null)
                {
                    Console.WriteLine("Failed to deserialize auth response");
                    return null;
                }

                Console.WriteLine("=== DESERIALIZED RESPONSE ==="); // Debug
                Console.WriteLine($"Token: {!string.IsNullOrEmpty(authResponse.Token)}");
                Console.WriteLine($"Email: {authResponse.Email}");
                Console.WriteLine($"UserId: {authResponse.UserId}");
                Console.WriteLine($"RoleId: {authResponse.RoleId}");
                Console.WriteLine($"RoleName: {authResponse.RoleName}");
                Console.WriteLine("===========================");

                // Decodeer JWT token om de naam te extraheren
                if (!string.IsNullOrEmpty(authResponse.Token))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(authResponse.Token);

                        // Extraheer naam uit JWT claims
                        var uniqueName = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                        if (!string.IsNullOrEmpty(uniqueName))
                        {
                            // Als je een Name property in AuthResponse hebt, kun je deze hier toewijzen
                            // authResponse.Name = uniqueName;
                            Console.WriteLine($"Found unique_name in JWT: {uniqueName}");
                        }
                    }
                    catch (Exception jwtEx)
                    {
                        Console.WriteLine($"Error decoding JWT: {jwtEx}");
                    }
                }

                return authResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AuthenticateAsync: {ex}");
                return null;
            }
        }
    }
}