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
                return RedirectToPage("/Admin/Products");
            }

            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/Product/{Id}");

            if (product == null)
            {
                return RedirectToPage("/Admin/Products");
            }

            Product = new ProductInputModel
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ExistingPictureBase64 = product.Picture != null ? Convert.ToBase64String(product.Picture) : null
            };


            return Page();
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

            content.Add(new StringContent(Product.ProductId.ToString()), "ProductId");
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
            else if (!string.IsNullOrEmpty(Product.ExistingPictureBase64))
            {
                var existingBytes = Convert.FromBase64String(Product.ExistingPictureBase64);
                var existingStreamContent = new ByteArrayContent(existingBytes);
                existingStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(existingStreamContent, "Picture", "existingImage.jpg");
            }

            var response = await _httpClient.PutAsync($"api/Product/{Product.ProductId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Fout bij het bijwerken van het product: {errorContent}");
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                return Page();
            }

            TempData["SuccessMessage"] = "Product succesvol bijgewerkt";
            return RedirectToPage("/Admin/Products");
        }

        public class ProductInputModel
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public IFormFile? Picture { get; set; }
            public string? ExistingPictureBase64 { get; set; }
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