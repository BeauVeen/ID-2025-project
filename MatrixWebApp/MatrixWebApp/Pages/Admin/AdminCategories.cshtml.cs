using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Admin
{
    public class AdminCategoriesModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AdminCategoriesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public CategoryDto NewCategory { get; set; }

        public List<CategoryDto> Categories { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("api/Category", NewCategory);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Fout bij toevoegen van de categorie.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var name = Request.Form["CategoryName"];

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(string.Empty, "Categorie naam is verplicht.");
                await OnGetAsync();
                return Page();
            }

            var category = new CategoryDto
            {
                CategoryId = id,
                CategoryName = name
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Category/{id}", category);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Fout bij bewerken van de categorie.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Category/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Fout bij verwijderen van de categorie.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
