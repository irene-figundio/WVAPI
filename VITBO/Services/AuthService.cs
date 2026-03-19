using System.Text;
using System.Text.Json;
using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            client.BaseAddress = new Uri(apiBase);

            var loginData = new { Username = model.Username, Password = model.Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return tokenResult?.Token;
            }

            return null;
        }

        private class TokenResponse
        {
            public string? Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
