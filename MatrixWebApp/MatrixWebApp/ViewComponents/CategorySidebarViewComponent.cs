using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace MatrixWebApp.ViewComponents
{
    public class CategorySidebarViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public CategorySidebarViewComponent(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("MatrixApi");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Category");
            return View(categories);
        }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}

