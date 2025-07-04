using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MatrixWebApp.Pages.Admin
{
    public class UserAuthorisationModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserAuthorisationModel> _logger;

        public List<UserViewModel> Users { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int BezorgersToShow { get; set; } = 5;

        [BindProperty(SupportsGet = true)]
        public int AdminsToShow { get; set; } = 5;
        public UserAuthorisationModel(IHttpClientFactory httpClientFactory, ILogger<UserAuthorisationModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
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
        public async Task OnGetAsync()
        {
            ViewData["ShowSidebar"] = true;
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/User");
                if (users != null)
                {
                    Users = users;
                }
                Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error retrieving users or roles from API.");
                Users = new List<UserViewModel>();
                Roles = new List<RoleDto>();
            }
        }

        public string GetRoleName(int roleId)
        {
            return Roles.Find(r => r.RoleId == roleId)?.RoleName ?? $"Onbekend ({roleId})";
        }

        public class UserViewModel
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
    }
}