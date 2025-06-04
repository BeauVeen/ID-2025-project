using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrixWebApp.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditUserModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public EditUserInputModel Input { get; set; }

        public List<RoleDto> Roles { get; set; } = new();

        public class RoleDto
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public class EditUserInputModel
        {
            public int UserId { get; set; }

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

            // Optional fields
            public string? Address { get; set; }
            public string? Zipcode { get; set; }
            public string? City { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");

            var user = await _httpClient.GetFromJsonAsync<EditUserInputModel>($"api/User/{id}");
            if (user == null)
                return NotFound();

            Input = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _httpClient.PutAsJsonAsync($"api/User/{Input.UserId}", Input);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Fout bij het bijwerken van de gebruiker.");
                return Page();
            }

            return RedirectToPage("/Admin/UserAuthorisation");
        }
    }
}