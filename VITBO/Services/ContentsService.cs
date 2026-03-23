using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class ContentsService : IContentsService
    {
        private readonly ApiService _apiService;
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;

        public ContentsService(ApiService apiService, HttpService httpService, IConfiguration configuration)
        {
            _apiService = apiService;
            _httpService = httpService;
            _configuration = configuration;
        }

        public async Task<bool> CreateContent(CreateContentRequest request,string token,string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Contents";
            var jsonContent = string.Empty ;
            jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
            //return await _apiService.PostVoidAsync("api/Contents", request);
        }

        public async Task<List<ContentDto>> GetContentsByTypeAsync(string contentType, int langId,string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Contents/type/{contentType}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentDto>>(result);
            return list ?? new List<ContentDto>();
        }

        public async Task<ContentDto?> GetContentByIdAsync(int id, int langId,string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Contents/{id}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var contentDto = await _httpService.GetBodyFromHttpResponseAsync<ContentDto>(result);
            return contentDto;
        }
         public async Task<bool> UpdateContentAsync(int id, ContentDto request,string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Contents/{id}";
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, token, userAgent, jsonRequest) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }
    }
}
