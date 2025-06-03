using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserAuthorisationModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserAuthorisationModel> _logger;

    public List<UserViewModel> Users { get; set; } = new();
    public List<RoleDto> Roles { get; set; } = new();

    public UserAuthorisationModel(IHttpClientFactory httpClientFactory, ILogger<UserAuthorisationModel> logger)
    {
        _httpClient = httpClientFactory.CreateClient("MatrixApi");
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        ViewData["ShowSidebar"] = true;
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/User");
            if (users != null)
            {
                Users = users;
            }
            Roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Role");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error retrieving users or roles from API.");
            Users = new List<UserViewModel>();
            Roles = new List<RoleDto>();
        }
    }

    public string GetRoleName(int roleId)
    {
        return Roles.Find(r => r.RoleId == roleId)?.RoleName ?? $"Onbekend ({roleId})";
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}