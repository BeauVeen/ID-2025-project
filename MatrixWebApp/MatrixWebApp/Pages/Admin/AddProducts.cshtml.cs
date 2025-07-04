using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
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

            // Debugging: Log the raw Price input
            Console.WriteLine($"Raw Price Input: {Product.Price}");

            // Ensure the Price is parsed correctly using the expected culture
            if (!decimal.TryParse(Product.Price.ToString(CultureInfo.CurrentCulture), NumberStyles.Number, new CultureInfo("nl-NL"), out var parsedPrice))
            {
                ModelState.AddModelError(nameof(Product.Price), "Ongeldige prijs. Gebruik een komma als decimaal scheidingsteken.");
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            // Debugging: Log the parsed Price
            Console.WriteLine($"Parsed Price: {parsedPrice}");

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(Product.CategoryId.ToString()), "CategoryId");
            content.Add(new StringContent(Product.Name ?? ""), "Name");
            content.Add(new StringContent(Product.Description ?? ""), "Description");
            // Format Price using invariant culture to ensure the API receives it correctly
            content.Add(new StringContent(parsedPrice.ToString("0.00", CultureInfo.InvariantCulture)), "Price");
            content.Add(new StringContent(Product.Stock.ToString()), "Stock");

            if (Product.Picture != null && Product.Picture.Length > 0)
            {
                var streamContent = new StreamContent(Product.Picture.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(Product.Picture.ContentType);
                content.Add(streamContent, "Picture", Product.Picture.FileName);
            }

            var response = await _httpClient.PostAsync("api/Product", content);

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
            public IFormFile? Picture { get; set;}
        }

        public class CategoryDto
        { 
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
