using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class UsersService : IUsersService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public UsersService(HttpService httpService,IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request,string sessionToken,string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Users";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken,request, userAgent);
            return response is HttpResponseMessage r && r.IsSuccessStatusCode;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
            }
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent,null);
            var result = await _httpService.GetBodyFromHttpResponseAsync<PagedResult<UserDto>>(response);
            return result ?? new PagedResult<UserDto> { Items = new List<UserDto>() };
        }
        public async Task<bool> DeleteUserAsync(Guid userId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Users/{userId}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, sessionToken, userAgent, null);
            return response is HttpResponseMessage r && r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Users/{userId}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, request, userAgent);
            return response is HttpResponseMessage r && r.IsSuccessStatusCode;
        }





        }

    public class UpdateUserRequest
    {
        public string? Email { get; set; }
        public bool? SuperAdmin { get; set; }
        public int? StatusId { get; set; }           // 0/1
    }
}
