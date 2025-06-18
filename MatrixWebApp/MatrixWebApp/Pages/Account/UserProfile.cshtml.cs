using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Net.Http;
using System.Threading.Tasks;

namespace MatrixWebApp.Pages.Account
{
    public class UserProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserProfileModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string UserName { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var client = _httpClientFactory.CreateClient("MatrixApi");
                var user = await client.GetFromJsonAsync<EditProfileModel.UserDto>($"api/User/{userId}");
                UserName = user?.Name ?? "User"; // Gebruik de naam uit de API, fallback naar "User"
            }
            else
            {
                UserName = "User";
            }
        }
    }
}