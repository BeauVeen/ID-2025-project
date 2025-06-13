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

                var allowedRoles = new[] { "administrator", "klant", "bezorger" };

                // Rolnaam veilig en consistent maken
                var roleNameLower = (result.roleName ?? "").Trim().ToLower();

                if (!allowedRoles.Contains(roleNameLower))
                {
                    Message = "Toegang geweigerd.";
                    return Page();
                }

                string roleName = char.ToUpper(roleNameLower[0]).ToString() + roleNameLower.Substring(1);

                // Claims maken voor cookie-authenticatie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.email),
                    new Claim(ClaimTypes.NameIdentifier, result.email),
                    new Claim("UserId", result.userId.ToString()),
                    new Claim(ClaimTypes.Role, roleName)
                };


                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                if (roleName == "Administrator")
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
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
            public int userId { get; set; }
            public int roleId { get; set; }
            public string roleName { get; set; }
        }


        public class ApiError
        {
            public string message { get; set; }
        }
    }
}