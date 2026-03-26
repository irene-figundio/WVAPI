using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApiService _apiService;

        public UsersService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request)
        {
            return await _apiService.PostVoidAsync("api/Users", request);
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page)
        {
            var endpoint = $"api/Users?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&query={Uri.EscapeDataString(query)}";
            }

            var result = await _apiService.GetAsync<PagedResult<UserDto>>(endpoint);
            return result ?? new PagedResult<UserDto> { Items = new List<UserDto>() };
        }
    }
}
