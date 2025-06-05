using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace MatrixWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            // leeg
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginPayload = new
            {
                email = Email,
                password = Password
            };

            var response = await _httpClient.PostAsJsonAsync("api/User/authenticate", loginPayload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                Console.WriteLine($"RoleName from API: {result.roleName}");

                // JWT opslaan in HttpOnly cookie (voor API calls)
                Response.Cookies.Append("jwt_token", result.token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
                });

                // Rolnaam veilig en consistent maken
                var roleName = (result.roleName ?? "User").Trim();

                // Optioneel: maak roleName case-insensitive en valideer
                if (!string.Equals(roleName, "Administrator", StringComparison.OrdinalIgnoreCase))
                {
                    roleName = "User";
                }
                else
                {
                    roleName = "Administrator";
                }

                // Claims maken voor cookie-authenticatie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.email),
                    new Claim(ClaimTypes.NameIdentifier, result.email),
                    new Claim(ClaimTypes.Role, roleName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToPage("/Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                     response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();
                Message = error?.message ?? "Ongeldige inloggegevens.";
                return Page();
            }
            else
            {
                Message = "Er is een fout opgetreden bij het inloggen.";
                return Page();
            }
        }

        public class LoginResult
        {
            public string token { get; set; }
            public string email { get; set; }
            public int roleId { get; set; }
            public string roleName { get; set; }
        }


        public class ApiError
        {
            public string message { get; set; }
        }
    }
}