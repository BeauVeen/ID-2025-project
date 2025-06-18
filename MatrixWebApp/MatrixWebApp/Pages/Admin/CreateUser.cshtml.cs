using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class CreateUserModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateUserModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public class RoleDto
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public List<RoleDto> Roles { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Naam")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Rol")]
            public int RoleId { get; set; }

            [Required]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; }

            // Optional fields
            public string? Address { get; set; }
            public string? Zipcode { get; set; }
            public string? City { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public async Task OnGetAsync()
        {
            ViewData["ShowSidebar"] = true;
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["ShowSidebar"] = true;
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var payload = new
            {
                name = Input.Name,
                email = Input.Email,
                roleId = Input.RoleId,
                password = Input.Password,
                address = Input.Address,
                zipcode = Input.Zipcode,
                city = Input.City,
                phoneNumber = Input.PhoneNumber,
                createdAt = DateTime.UtcNow 
            };

            var response = await _httpClient.PostAsJsonAsync("api/User", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Admin/UserAuthorisation");
            }

            ModelState.AddModelError(string.Empty, "Fout bij het aanmaken van de gebruiker.");
            return Page();
        }
    }
}