using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        public string SuccessMessage { get; private set; }

        public void OnGet()
        {
            SuccessMessage = TempData["SuccessMessage"] as string;
        }
    }
}
