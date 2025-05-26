using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MatrixWebApp.Pages
{
    public class AdminModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public ProductDto Product { get; set; } = new();

        public List<CategorieDto> Categorieën { get; set; } = new();

        public string Melding { get; set; }

        public AdminModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public async Task OnGetAsync()
        {
            Categorieën = await _httpClient.GetFromJsonAsync<List<CategorieDto>>("api/Categorie");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Gekozen CategorieId: {Product.CategorieId}");
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"{entry.Key}: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                Categorieën = await _httpClient.GetFromJsonAsync<List<CategorieDto>>("api/Categorie");
                Melding = "Controlleer je input";
                return Page();
            }

            var response = await _httpClient.PostAsJsonAsync("api/Product", Product);

            if (response.IsSuccessStatusCode)
            {
                Melding = "Product succesvol toegevoegd";
                ModelState.Clear();
                Product = new ProductDto();
            }
            else
            {
                Melding = $"Fout bij het toevoegen van product: {response.StatusCode}";
            }

            Categorieën = await _httpClient.GetFromJsonAsync<List<CategorieDto>>("api/Categorie");
            return Page();
        }

        public class CategorieDto
        {
            [Required(ErrorMessage = "Categorie is verplicht")]
            public int CategorieId { get; set; }
            [Required(ErrorMessage = "Categorie naam is verplicht")]
            public string CategorieNaam { get; set; }
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            [Required(ErrorMessage = "Categorie is verplicht")]
            public int? CategorieId { get; set; }
            [Required(ErrorMessage = "Naam is verplicht")]
            public string Naam { get; set; }
            public string Beschrijving { get; set; }
            [Range(0.01, 999999999.99, ErrorMessage ="Prijs moet minimaal 0.01 zijn")]
            public decimal Prijs { get; set; }
            [Range(0, int.MaxValue, ErrorMessage = "Voorraad kan niet negatief zijn")]
            public int Voorraad { get; set; }
            public byte[]? Afbeelding { get; set; }
        }
    }
}
