using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IUsersService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(string? query, int page);
        Task<bool> CreateUserAsync(CreateUserRequest request);
    }
}
