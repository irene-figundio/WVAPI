using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;

        public UsersService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request, string sessionToken, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/Users";
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, userAgent, jsonRequest);
            return response is HttpResponseMessage r && r.IsSuccessStatusCode;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string sessionToken, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
            }

            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent, null);
            var result = await _httpService.GetBodyFromHttpResponseAsync<PagedResult<UserDto>>(response);
            return result ?? new PagedResult<UserDto> { Items = new List<UserDto>() };
        }
    }
}
