using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MatrixWebApp.Pages.Admin.AddProductsModel;

namespace MatrixWebApp.Pages.Admin
{
    public class EditProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditProductsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        [BindProperty]
        public ProductInputModel Product { get; set; }

        public List<CategoryDto> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");

            if (Id == 0)
            {
                return RedirectToPage("/Admin/AdminProducts");
            }

            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/Product/{Id}");

            if (product == null)
            {
                return RedirectToPage("/Admin/AdminProducts");
            }

            Product = new ProductInputModel
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"POST ontvangen voor product ID: {Id}");

            if (!ModelState.IsValid)
            {
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            Console.WriteLine($"Product.ProductId: {Product.ProductId}, Id: {Id}");

            var updatePayload = new
            {
                ProductId = Product.ProductId,
                CategoryId = Product.CategoryId,
                Name = Product.Name,
                Description = Product.Description,
                Price = Product.Price,
                Stock = Product.Stock,
                Picture = (byte[])null //Aanpassen als foto werkt.
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Product/{Id}", updatePayload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Fout bij het bijwerken van het product: {errorContent}");
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            TempData["SuccessMessage"] = "Product succesvol bijgewerkt";
            return RedirectToPage("/Admin/AdminProducts");
        }

        public class ProductInputModel
        {
            public int ProductId { get; set; }
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
    }
}