using VITBO.Models;
using VITBO.Services.Interfaces;
using System.Text.Json;

namespace VITBO.Services
{
    public class MediaService : IMediaService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;

        public MediaService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
        }

        public async Task<List<ContentImageDto>> GetContentImagesAsync(int? contentId, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentimages";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentImageDto>>(result);
            if (contentId.HasValue) {
                return list?.Where(x => x.ContentId == contentId.Value).ToList() ?? new List<ContentImageDto>();
            }
            return list ?? new List<ContentImageDto>();
        }

        public async Task<bool> CreateContentImageAsync(ContentImageDto model, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentimages";
            var jsonContent = JsonSerializer.Serialize(model);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContentImageAsync(int id, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentimages/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<GalleryDto>> GetGalleriesAsync(int? eventId, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/galleries";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<GalleryDto>>(result);
            if (eventId.HasValue) {
                return list?.Where(x => x.EventId == eventId.Value).ToList() ?? new List<GalleryDto>();
            }
            return list ?? new List<GalleryDto>();
        }

        public async Task<bool> CreateGalleryAsync(GalleryDto model, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/galleries";
            var jsonContent = JsonSerializer.Serialize(model);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteGalleryAsync(int id, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/galleries/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<PhotoGalleryDto>> GetPhotosAsync(int? galleryId, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/photogallery";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<PhotoGalleryDto>>(result);
            if (galleryId.HasValue) {
                return list?.Where(x => x.GalleryId == galleryId.Value).ToList() ?? new List<PhotoGalleryDto>();
            }
            return list ?? new List<PhotoGalleryDto>();
        }

        public async Task<bool> CreatePhotoAsync(PhotoGalleryDto model, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/photogallery";
            var jsonContent = JsonSerializer.Serialize(model);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePhotoAsync(int id, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/photogallery/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<ContentExpertDto>> GetContentExpertsAsync(int contentId, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentexperts";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentExpertDto>>(result);
            return list?.Where(x => x.ContentId == contentId).ToList() ?? new List<ContentExpertDto>();
        }

        public async Task<bool> AddContentExpertAsync(ContentExpertDto model, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentexperts";
            var jsonContent = JsonSerializer.Serialize(model);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveContentExpertAsync(int id, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/contentexperts/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<EventExpertDto>> GetEventExpertsAsync(int eventId, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/eventexperts";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventExpertDto>>(result);
            return list?.Where(x => x.EventId == eventId).ToList() ?? new List<EventExpertDto>();
        }

        public async Task<bool> AddEventExpertAsync(EventExpertDto model, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/eventexperts";
            var jsonContent = JsonSerializer.Serialize(model);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, userAgent, jsonContent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveEventExpertAsync(int id, string token, string userAgent)
        {
            var apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
            var endpoint = $"{apiBase}/api/eventexperts/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }
    }
}
