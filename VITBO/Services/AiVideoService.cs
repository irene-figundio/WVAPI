using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class AiVideoService : IAiVideoService
    {
        private readonly ApiService _apiService;
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;

        public AiVideoService(ApiService apiService, HttpService httpService, IConfiguration configuration)
        {
            _apiService = apiService;
            _httpService = httpService;
            _configuration = configuration;
        }

        public async Task<bool> CreateVideoAsync(CreateVideoRequest request, string sessionToken, string userAgent, CancellationToken ct)
        {
            //var response = await _apiService.PostAsync("api/Video", request, sessionToken, userAgent, ct);
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
                var body = new
                {
                    Title = request.Title,
                    Url_Video = request.Url_Video,
                    IsLandscape = request.IsLandscape,
                    Play_Priority = request.Play_Priority,
                    ID_Session = request.ID_Session
                };
            var jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post,$"{apiBase}api/Video", sessionToken, jsonBody, userAgent, ct);
            if (response == null)
            {
                return false;
            }
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);               
                //_apiService.LogError($"Failed to create video. Status: {response.StatusCode}, Response: {errorContent}");
                return false;
            }
            //  return await _apiService.PostVoidAsync("api/Video", request);
        }

        public async Task<PagedResult<VideoListItemDto>> GetVideosAsync(string? query, int page, string sessionToken, string userAgent, CancellationToken ct)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}api/Video?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&q={Uri.EscapeDataString(query)}";
            }

            var result = await _apiService.GetAsync<PagedResult<VideoListItemDto>>(endpoint);
            return result ?? new PagedResult<VideoListItemDto> { Items = new List<VideoListItemDto>() };
        }
    }
}
