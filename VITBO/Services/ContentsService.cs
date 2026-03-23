using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class ContentsService : IContentsService
    {
        private readonly ApiService _apiService;

        public ContentsService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> CreateContent(CreateContentRequest request)
        {
            return await _apiService.PostVoidAsync("api/Contents", request);
        }

        public async Task<List<ContentDto>> GetContentsByTypeAsync(string contentType, int langId)
        {
            var endpoint = $"api/Contents/type/{contentType}?langId={langId}";
            var result = await _apiService.GetAsync<List<ContentDto>>(endpoint);
            return result ?? new List<ContentDto>();
        }
    }
}
