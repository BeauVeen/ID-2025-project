using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
            //leeg
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
                // Store the JWT token in a cookie (HttpOnly for security)
                Response.Cookies.Append("jwt_token", result.token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
                });

                Message = "Succesvol ingelogd!";
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
        }

        public class ApiError
        {
            public string message { get; set; }
        }
    }
}