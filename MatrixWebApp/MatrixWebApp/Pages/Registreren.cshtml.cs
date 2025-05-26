using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages
{
    public class RegistrerenModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public RegistrerenModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public GebruikerDto Gebruiker { get; set; }

        [BindProperty]
        public string Wachtwoord { get; set; }
        public string Foutmelding { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Gebruiker.Wachtwoord = HashWachtwoord(Wachtwoord);

            var response = await _clientFactory.CreateClient("MatrixApi").PostAsJsonAsync("api/gebruikers", Gebruiker);

            if (response.IsSuccessStatusCode)
            { 
                return RedirectToPage("/Login");
            }
            else
            {
                var inhoud = await response.Content.ReadAsStringAsync();
                Foutmelding = $"Registratie mislukt {inhoud}";
                return Page();
            }

                
        }

        private string HashWachtwoord(string wachtwoord)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(wachtwoord));
            return Convert.ToBase64String(bytes);
        }

        public class GebruikerDto
        {
            public string Wachtwoord { get; set; }
            public string Naam { get; set; }
            public string Adres { get; set; }
            public string Postcode { get; set; }
            public string Woonplaats { get; set; }
            public string Telefoon { get; set; }
            public string Email { get; set; }
        }
    }
}
