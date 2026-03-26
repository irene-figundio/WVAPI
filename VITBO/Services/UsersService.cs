using System.Runtime.Intrinsics.Arm;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApiService _apiService;
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly Uri _apiBaseAddress;

        public UsersService(ApiService apiService, HttpService httpService, IConfiguration configuration)
        {
            _apiService = apiService;
            _httpService = httpService;
            _configuration = configuration;
            var baseAddress = configuration["ApiBaseAddress"];
            _apiBaseAddress = new Uri(!string.IsNullOrWhiteSpace(baseAddress)
                ? baseAddress
                : "https://localhost:7275");
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseAddress}Users";
            //var jsonContent = string.Empty;
            //jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseAddress}Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
            }

            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var pagedResult = await _httpService.GetBodyFromHttpResponseAsync<PagedResult<UserDto>>(result);
            return pagedResult ?? new PagedResult<UserDto> { Items = new List<UserDto>() };
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto request, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseAddress}Users/{id}";
           // var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, token, request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseAddress}Users/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<UserDto> GetUserByIdAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBaseAddress}Users/{id}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var userDto = await _httpService.GetBodyFromHttpResponseAsync<UserDto>(result);
            return userDto ?? new UserDto();
        }
    }
}
