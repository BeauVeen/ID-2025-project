using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class CustomersModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CustomersModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public List<UserDto> Users { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();

        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int RoleId { get; set; }
        }

        public class RoleDto
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public async Task OnGetAsync()
        {
            Users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/User");
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");
        }

        public string GetRoleName(int roleId)
        {
            return Roles.FirstOrDefault(r => r.RoleId == roleId)?.RoleName ?? "";
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/User/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Fout bij verwijderen van de gebruiker.");
            }
            return RedirectToPage();
        }
    }
}