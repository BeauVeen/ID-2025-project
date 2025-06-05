using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Account
{
    public class CreateUserModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateUserModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public CreateUserDto Input { get; set; }

        public string StatusMessage { get; set; }

        public void OnGet(string statusMessage = null)
        {
            StatusMessage = statusMessage;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7113/api/User", Input);

            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Gebruiker succesvol aangemaakt.";
                return RedirectToPage("/Account/CreateUser", new { statusMessage = StatusMessage });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Fout bij aanmaken gebruiker: {errorContent}");
                return Page();
            }
        }


        public class CreateUserDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Address { get; set; }
            public string Zipcode { get; set; }
            public string City { get; set; }
            public string PhoneNumber { get; set; }
        }
    }
}
