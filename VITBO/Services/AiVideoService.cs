using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class AiVideoService : IAiVideoService
    {
        private readonly ApiService _apiService;

        public AiVideoService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> CreateVideoAsync(CreateVideoRequest request)
        {
            return await _apiService.PostVoidAsync("api/Video", request);
        }

        public async Task<PagedResult<VideoListItemDto>> GetVideosAsync(string? query, int page)
        {
            var endpoint = $"api/Video?page={page}&pageSize=20";
            if (!string.IsNullOrEmpty(query))
            {
                endpoint += $"&q={Uri.EscapeDataString(query)}";
            }

            var result = await _apiService.GetAsync<PagedResult<VideoListItemDto>>(endpoint);
            return result ?? new PagedResult<VideoListItemDto> { Items = new List<VideoListItemDto>() };
        }
    }
}
