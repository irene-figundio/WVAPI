using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace VITBO.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(configuration["ApiBaseAddress"] ?? "https://localhost:7275");
            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Attach JWT token from claims or session to the HttpClient if it exists
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("JWToken")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            }
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
            }
            return default;
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
            }
            return default;
        }

        public async Task<bool> PostVoidAsync(string endpoint, object data)
        {           
            var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, stringContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync(string endpoint, object data)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, stringContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}
