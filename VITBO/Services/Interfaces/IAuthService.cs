using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginViewModel model);
    }
}
