using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MatrixWebApp.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsModel> _logger;

        public List<ProductDto> Products { get; set; } = new();

        public ProductsModel(IHttpClientFactory httpClientFactory, ILogger<ProductsModel> logger)
        { 
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Product");

                if (products != null)
                {
                    Products = products;
                }
                else
                {
                    _logger.LogWarning("No products received from API.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error with retrieving products from API.");
            }
        }


        public class ProductDto
        { 
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int Stock {  get; set; }
            public byte[] Picture { get; set; }
        }
    }
}
