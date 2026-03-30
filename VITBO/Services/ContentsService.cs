using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class ContentsService : IContentsService
    {
        
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public ContentsService( HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<bool> CreateContent(CreateContentRequest request,string token,string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Contents";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token,request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
            //return await _apiService.PostVoidAsync("api/Contents", request);
        }

        public async Task<List<ContentDto>> GetContentsByTypeAsync(string contentType, int langId,string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Contents/type/{contentType}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentDto>>(result);
            return list ?? new List<ContentDto>();
        }

        public async Task<ContentDto?> GetContentByIdAsync(int id, int langId,string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Contents/{id}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var contentDto = await _httpService.GetBodyFromHttpResponseAsync<ContentDto>(result);
            return contentDto;
        }
         public async Task<bool> UpdateContentAsync(int id, ContentDto request,string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Contents/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, token,request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContentAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/Contents/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<ContentCategoryDto>> GetContentCategoriesAsync(int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/contentcategories?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentCategoryDto>>(result);
            return list ?? new List<ContentCategoryDto>();
        }
    }
}
