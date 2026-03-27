using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpService _apiService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public UsersService(HttpService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiBaseAddress"];
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request,string token,string userAgent)
        {
            var endpoint = $"{_apiBaseUrl}/Users";
            return await _apiService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode; ;
            
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseUrl}/Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
            }

            var result = await _apiService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            return await _apiService.GetBodyFromHttpResponseAsync<PagedResult<UserDto>>(result) ?? new PagedResult<UserDto> { Items = new List<UserDto>() };
        }
    }
}
