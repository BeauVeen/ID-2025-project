using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MatrixWebApp.Pages.Admin
{
    public class AddProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AddProductsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public ProductInputModel Product { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            var productPayload = new
            {
                CategoryId = Product.CategoryId,
                Name = Product.Name,
                Description = Product.Description,
                Price = Product.Price,
                Stock = Product.Stock,
                Picture = (byte[])null //Tijdelijk NULL, aangezien moeite crashes.
            };

            var response = await _httpClient.PostAsJsonAsync("api/Product", productPayload);

            if (response.IsSuccessStatusCode)
            {
                Product = new ProductInputModel();
                SuccessMessage = "Product succesvol toegevoegd";
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "Fout bij het toevoegen van het product.");
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            return Page();
        }

        public class ProductInputModel
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        public class CategoryDto
        { 
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
