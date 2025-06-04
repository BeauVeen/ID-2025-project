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

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<RoleDto> Roles { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Naam is verplicht")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email is verplicht")]
            [EmailAddress(ErrorMessage = "Ongeldig emailadres")]
            public string Email { get; set; }

            [Display(Name = "Wachtwoord")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "Rol is verplicht")]
            public int? RoleId { get; set; }

            public string? Address { get; set; }
            public string? Zipcode { get; set; }
            public string? City { get; set; }

            [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
            public string? PhoneNumber { get; set; }
        }

        public class RoleDto
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int RoleId { get; set; }
            public string? Address { get; set; }
            public string? Zipcode { get; set; }
            public string? City { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Password { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");

            var user = await _httpClient.GetFromJsonAsync<UserDto>($"api/User/{Id}");
            if (user == null)
                return RedirectToPage("/Admin/UserAuthorisation");

            Input = new InputModel
            {
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                Address = user.Address,
                Zipcode = user.Zipcode,
                City = user.City,
                PhoneNumber = user.PhoneNumber
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");

            if (!ModelState.IsValid)
                return Page();

            var updatePayload = new Dictionary<string, object>
            {
                ["userId"] = Id,
                ["name"] = Input.Name,
                ["email"] = Input.Email,
                ["roleId"] = Input.RoleId,
                ["address"] = Input.Address,
                ["zipcode"] = Input.Zipcode,
                ["city"] = Input.City,
                ["phoneNumber"] = Input.PhoneNumber
            };

           
            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                updatePayload["password"] = Input.Password;
            }

            var response = await _httpClient.PutAsJsonAsync($"api/User/{Id}", updatePayload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Fout bij het bijwerken van de gebruiker: {errorContent}");
                return Page();
            }

            TempData["SuccessMessage"] = "Gebruiker succesvol bijgewerkt";
            return RedirectToPage("/Admin/UserAuthorisation");
        }
    }
}