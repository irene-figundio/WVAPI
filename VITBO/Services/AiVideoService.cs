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
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Video/upload";

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.Title), "Title");
            content.Add(new StringContent(request.IsLandscape.ToString().ToLower() ?? "false"), "isLandscape");
            //content.Add(new StringContent(request.ID_Session.ToString()), "idUser"); // Backend endpoint maps ID_Session to idUser for this endpoint

            if (request.File != null)
            {
                var streamContent = new StreamContent(request.File.OpenReadStream());
                content.Add(streamContent, "file", request.File.FileName);
            }

            var response = await _httpService.PostMultipartAsync(endpoint, content, sessionToken, userAgent, ct);

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
                return false;
            }
        }

        public async Task<bool> UpdateVideoAsync(int id, UpdateVideoRequest request, string sessionToken, string userAgent, CancellationToken ct)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Video/{id}";

            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, request, userAgent, ct);

            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVideoAsync(int id, string sessionToken, string userAgent, CancellationToken ct)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Video/{id}";

            var payload = new { IsDeleted = true };
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, payload, userAgent, ct);

            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<PagedResult<VideoListItemDto>> GetVideosAsync(string? query, int page, string sessionToken, string userAgent, CancellationToken ct)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}Video/all";

            // Pass the session token and user agent as requested
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, null, userAgent, ct);
            if (response == null || !response.IsSuccessStatusCode)
            {
                return new PagedResult<VideoListItemDto> { Items = new List<VideoListItemDto>(), Total = 0, Page = page, PageSize = 20 };
            }

            var allItems = await _httpService.GetBodyFromHttpResponseAsync<List<VideoListItemDto>>(response);
            if (allItems == null)
            {
                return new PagedResult<VideoListItemDto> { Items = new List<VideoListItemDto>(), Total = 0, Page = page, PageSize = 20 };
            }

            // Apply search filter locally since /all endpoint might not support `q` parameter
            if (!string.IsNullOrEmpty(query))
            {
                allItems = allItems.Where(v => v.Title != null && v.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply pagination locally
            var total = allItems.Count;
            var pagedItems = allItems.Skip((page - 1) * 20).Take(20).ToList();

            return new PagedResult<VideoListItemDto>
            {
                Items = pagedItems,
                Total = total,
                Page = page,
                PageSize = 20
            };
        }
    }
}
