using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace MatrixWebApp.Pages.Account
{
    [Authorize(Roles = "Klant")]
    public class EditProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditProfileModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Naam is verplicht")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email is verplicht")]
            [EmailAddress(ErrorMessage = "Ongeldig emailadres")]
            public string Email { get; set; }

            [Display(Name = "Wachtwoord")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [Required(ErrorMessage = "Huidig wachtwoord invoeren ter bevestiging")]
            [DataType(DataType.Password)]
            [Display(Name = "Huidig wachtwoord")]
            public string CurrentPassword { get; set; }

            [Display(Name = "Adres")]
            public string? Address { get; set; }

            [Display(Name = "Postcode")]
            public string? Zipcode { get; set; }

            [Display(Name = "Stad")]
            public string? City { get; set; }

            [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
            public string? PhoneNumber { get; set; }
        }

        private HttpClient GetAuthorizedHttpClient()
        {
            var client = _httpClientFactory.CreateClient("MatrixApi");

            // JWT token ophalen uit de cookie
            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt_token"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = GetAuthorizedHttpClient();

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await client.GetFromJsonAsync<UserDto>($"api/User/{userId}");
            if (user == null)
                return NotFound();

            Input = new InputModel
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Zipcode = user.Zipcode,
                City = user.City,
                PhoneNumber = user.PhoneNumber,

            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = GetAuthorizedHttpClient();

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var authRequest = new { email = Input.Email, password = Input.CurrentPassword };
            var authResponse = await client.PostAsJsonAsync("api/User/authenticate", authRequest);

            if (!authResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Authenticatie mislukt: verkeerd wachtwoord.");
                return Page();
            }

            var updatePayload = new Dictionary<string, object>
            {
                ["userId"] = userId,
                ["name"] = Input.Name,
                ["email"] = Input.Email,
                ["address"] = Input.Address,
                ["zipcode"] = Input.Zipcode,
                ["city"] = Input.City,
                ["phoneNumber"] = Input.PhoneNumber,
                ["roleId"] = 1
            };

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                updatePayload["passwordHash"] = Input.Password;
            }

            var response = await client.PutAsJsonAsync($"api/User/{userId}", updatePayload);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Update response content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"Fout bij het bijwerken van je profiel: {responseContent}");
                return Page();
            }

            TempData["SuccessMessage"] = "Je profiel is bijgewerkt";
            return RedirectToPage();
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string? Address { get; set; }
            public string? Zipcode { get; set; }
            public string? City { get; set; }
            public string? PhoneNumber { get; set; }
        }
    }
}
