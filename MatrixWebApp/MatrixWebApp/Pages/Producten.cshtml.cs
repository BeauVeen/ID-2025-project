using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MatrixWebApp.Pages
{
    public class ProductenModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ProductDto> Producten { get; set; } = new();

        public ProductenModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public async Task OnGetAsync()
        {
            var producten = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");
            if (producten != null)
            {
                Producten = producten;
            }
        }

        public class ProductDto
        {
            public int ProductId { get; set; }
            public int? CategorieId { get; set; }
            public string Naam { get; set; }
            public string Beschrijving { get; set; }
            public decimal Prijs { get; set; }
            public int Voorraad { get; set; }
            public byte[] Afbeelding { get; set; }
        }
    }
}
