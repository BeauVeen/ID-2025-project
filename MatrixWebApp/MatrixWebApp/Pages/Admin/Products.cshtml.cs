using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class AdminProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsModel> _logger;

        public List<ProductDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ProductDto> FilteredProducts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchSerial { get; set; }

        public string SuccessMessage { get; set; }

        public AdminProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");
                if (products != null)
                {
                    Products = products;
                    FilteredProducts = products;
                }

                var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                if (categories != null)
                {
                    Categories = categories;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error with retrieving products from API.");
                FilteredProducts = new List<ProductDto>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Product/{id}");

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Product succesvol verwijderd";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fout bij verwijderen van het product.");
            }

            Products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product") ?? new List<ProductDto>();
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category") ?? new List<CategoryDto>();
            return Page();
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public byte[] Picture { get; set; }
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
