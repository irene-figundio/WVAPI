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
        private readonly string _apiBase;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<string?> LoginAsync(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_apiBase);

            var loginData = $"{{ \"username\" : {model.Username},  \"password \" : {model.Password} }}";
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{client.BaseAddress}Auth/login", content);
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
