using AI_Integration.DataAccess;

namespace AI_Integration.Helpers
{
    public interface IAuthService
    {
        // Restituisce lo stesso payload di /api/Auth/token: { token: "<JWT>" }
        Task<(string Token, DateTime Expires)> LoginWithCredentialsAsync(LoginRequest loginRequest, string? userAgent);
        
    }

}
