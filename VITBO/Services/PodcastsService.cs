using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class PodcastsService : IPodcastsService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public PodcastsService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<List<PodcastDto>> GetPodcastsAsync(int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/podcasts?langId={langId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<PodcastDto>>(response);
            return list ?? new List<PodcastDto>();
        }

        public async Task<int?> CreatePodcastAsync(CreatePodcastRequest request, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/podcasts";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, request, userAgent);
            if (response != null && response.IsSuccessStatusCode)
            {
                var body = await _httpService.GetBodyFromHttpResponseAsync<dynamic>(response);
                if (body != null && body.GetProperty("id").ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    return body.GetProperty("id").GetInt32();
                }
            }
            return null;
        }

        public async Task<PodcastDto?> GetPodcastByIdAsync(int id, int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/podcasts/{id}?langId={langId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<PodcastDto>(response);
        }

        public async Task<bool> UpdatePodcastAsync(int id, PodcastDto request, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/podcasts/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, token, request, userAgent);
            return response?.IsSuccessStatusCode ?? false;
        }

        public async Task<bool> DeletePodcastAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/podcasts/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent);
            return response?.IsSuccessStatusCode ?? false;
        }
    }
}
