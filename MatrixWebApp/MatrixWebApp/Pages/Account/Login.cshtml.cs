using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            // leeg of init
        }

        public IActionResult OnPostLogin()
        {
            // Simpel voorbeeld: check vast email en wachtwoord
            if (Email == "test@example.com" && Password == "wachtwoord")
            {
                Message = "Succesvol ingelogd!";
                // hier kan je redirect doen of sessie starten, etc.
            }
            else
            {
                Message = "Ongeldige inloggegevens.";
            }

            return Page();
        }
    }
}
