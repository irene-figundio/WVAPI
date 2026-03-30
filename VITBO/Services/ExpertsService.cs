using VITBO.Models;
using VITBO.Services.Interfaces;
using System.Text.Json;

namespace VITBO.Services
{
    public class ExpertsService : IExpertsService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public ExpertsService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<List<ExpertDto>> GetExpertsAsync(int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/experts?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ExpertDto>>(result);
            return list ?? new List<ExpertDto>();
        }

        public async Task<ExpertDto?> GetExpertByIdAsync(int id, int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/experts/{id}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            return await _httpService.GetBodyFromHttpResponseAsync<ExpertDto>(result);
        }

        public async Task<bool> CreateExpertAsync(ExpertDto expert, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/experts";
           // var jsonContent = JsonSerializer.Serialize(expert);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token,expert, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateExpertAsync(int id, ExpertDto expert, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/experts/{id}";
            //var jsonContent = JsonSerializer.Serialize(expert);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, token,expert, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteExpertAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/experts/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }
    }
}
