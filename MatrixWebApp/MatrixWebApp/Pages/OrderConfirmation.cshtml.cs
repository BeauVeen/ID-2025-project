using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;

namespace MatrixWebApp.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public OrderConfirmationModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MatrixApi");
        }

        public OrderResponse Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            Order = await _httpClient.GetFromJsonAsync<OrderResponse>($"api/Order/{orderId}");

            if (Order == null)
            {
                return NotFound();
            }

            return Page();
        }

        public class OrderResponse
        {
            public int OrderId { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}