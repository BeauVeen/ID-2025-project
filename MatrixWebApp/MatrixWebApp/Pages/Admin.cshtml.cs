using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MatrixWebApp.Pages
{
    public class AdminModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public ProductDto Product { get; set; } = new ProductDto();

        public List<CategoryDto> Categories { get; set; } = new();

        public string Melding { get; set; }

        public List<ProductDto> LageVoorraadProducten { get; set; } = new();

        public AdminModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public async Task OnGetAsync()
        {
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            var producten = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

            LageVoorraadProducten = producten
                .Where(p => p.Stock < 10)
                .ToList();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
                var producten = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");
                LageVoorraadProducten = producten
                    .Where(p => p.Stock < 10)
                    .ToList();

                Melding = "Controleer je input";
                return Page();
            }

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(Product.Naam ?? ""), "Naam");
            form.Add(new StringContent(Product.Beschrijving ?? ""), "Beschrijving");
            form.Add(new StringContent(Product.Prijs.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Prijs");
            form.Add(new StringContent(Product.Stock.ToString()), "Stock");
            form.Add(new StringContent(Product.CategoryId?.ToString() ?? ""), "CategoryId");

            if (Product.Afbeelding != null)
            {
                var byteArrayContent = new ByteArrayContent(Product.Afbeelding);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                form.Add(byteArrayContent, "Afbeelding", "upload.jpg");
            }

            var response = await _httpClient.PostAsync("api/Product", form);

            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            var alleProducten = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

            LageVoorraadProducten = alleProducten
                .Where(p => p.Stock < 10)
                .ToList();

            if (response.IsSuccessStatusCode)
            {
                Melding = "Product succesvol toegevoegd";
                Product = new ProductDto(); // reset form
                ModelState.Clear();
            }
            else
            {
                Melding = $"Fout bij het toevoegen van product: {response.StatusCode}";
            }

            return Page();
        }


        public class CategoryDto
        {
            [Required(ErrorMessage = "Category is verplicht")]
            public int CategoryId { get; set; }

            [Required(ErrorMessage = "Category naam is verplicht")]
            public string CategoryName { get; set; }
        }

        public class ProductDto
        {
            public ProductDto()
            {
                Prijs = 1.00m;
                Stock = 1;   // Stock property gebruiken
            }

            public int ProductId { get; set; }

            [Required(ErrorMessage = "Category is verplicht")]
            public int? CategoryId { get; set; }

            [Required(ErrorMessage = "Naam is verplicht")]
            public string Naam { get; set; }

            public string Beschrijving { get; set; }

            [Range(0.01, 999999999.99, ErrorMessage = "Prijs moet minimaal 0.01 zijn")]
            public decimal Prijs { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Voorraad kan niet negatief zijn")]
            public int Stock { get; set; }  // Stock ipv Voorraad

            public byte[]? Afbeelding { get; set; }
        }
    }
}
