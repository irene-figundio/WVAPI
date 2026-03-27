using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IUsersService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page, string token, string userAgent);
        Task<bool> CreateUserAsync(CreateUserRequest request, string token, string userAgent);
    }
}
