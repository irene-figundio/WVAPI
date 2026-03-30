using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IUsersService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string sessionToken, string userAgent);
        Task<bool> CreateUserAsync(CreateUserRequest request, string sessionToken, string userAgent);
    }
}
