using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        // Uitloggen van cookie authentication
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // JWT-cookie verwijderen (indien gebruikt)
        if (Request.Cookies.ContainsKey("jwt_token"))
        {
            Response.Cookies.Delete("jwt_token");
        }

        // Redirect naar home of login pagina
        return RedirectToPage("/Index"); // of "/Account/Login"
    }
}
