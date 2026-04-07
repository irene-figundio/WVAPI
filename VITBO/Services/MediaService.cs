using VITBO.Models;
using VITBO.Services.Interfaces;
using System.Text.Json;
using System.Net.Http.Headers;

namespace VITBO.Services
{
    public class MediaService : IMediaService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public MediaService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _baseUrl = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<List<ContentImageDto>> GetContentImagesAsync(string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentimages";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentImageDto>>(result);
            return list ?? new List<ContentImageDto>();
        }

        public async Task<List<ContentImageDto>> GetContentImagesByContentIdAsync(int? contentId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentimages/bycontent/{contentId}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentImageDto>>(result);
            return list ?? new List<ContentImageDto>();
        }

        public async Task<bool> CreateContentImageAsync(ContentImageDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentimages";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContentImageAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentimages/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<GalleryDto>> GetGalleriesAsync(string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/galleries";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<GalleryDto>>(result);
            return list ?? new List<GalleryDto>();
        }

        public async Task<GalleryDto> GetGalleriesByEventIdAsync(int? eventId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/galleries/byevent/{eventId}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<GalleryDto>(result);
            return list ?? new GalleryDto();
        }

        public async Task<bool> CreateGalleryAsync(GalleryDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/galleries";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteGalleryAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/galleries/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<PhotoGalleryDto>> GetPhotosAsync(string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/photogallery";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<PhotoGalleryDto>>(result);
            return list ?? new List<PhotoGalleryDto>();
        }
        public async Task<List<PhotoGalleryDto>> GetPhotosByGalleryIdAsync(int? galleryId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/photogallery/bygallery/{galleryId}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<PhotoGalleryDto>>(result);
            return list ?? new List<PhotoGalleryDto>();
        }

        public async Task<PhotoGalleryDto> GetPhotoByIdAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/photogallery/{id}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var item = await _httpService.GetBodyFromHttpResponseAsync<PhotoGalleryDto>(result);
            return item ?? new PhotoGalleryDto();
        }

        public async Task<bool> CreatePhotoAsync(PhotoGalleryDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/photogallery";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePhotoAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/photogallery/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<ContentExpertDto>> GetContentExpertsAsync(int contentId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/experts/bycontent/{contentId}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentExpertDto>>(result);
            return list?.ToList() ?? new List<ContentExpertDto>();
        }

        public async Task<bool> AddContentExpertAsync(ContentExpertDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentexperts";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveContentExpertAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentexperts/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<EventExpertDto>> GetEventExpertsAsync(int eventId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/experts/byevent/{eventId}";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventExpertDto>>(result);
            return list?.ToList() ?? new List<EventExpertDto>();
        }

        public async Task<bool> AddEventExpertAsync(EventExpertDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/eventexperts";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveEventExpertAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/eventexperts/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<ContentLinkDto>> GetContentLinksAsync(int? contentId, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentlinks";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<ContentLinkDto>>(result);
            if (contentId.HasValue)
            {
                return list?.Where(x => x.ContentId == contentId.Value).ToList() ?? new List<ContentLinkDto>();
            }
            return list ?? new List<ContentLinkDto>();
        }

        public async Task<bool> CreateContentLinkAsync(ContentLinkDto model, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentlinks";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContentLinkAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/contentlinks/{id}";
            return await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        // Implementation of new methods
        public async Task<UploadResponseDto> UploadFileAsync(MultipartFormDataContent content, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/upload";
            var response = await _httpService.PostMultipartAsync(endpoint, content, token, userAgent);
            var result = await _httpService.GetBodyFromHttpResponseAsync<UploadResponseDto>(response);
            return result ?? new UploadResponseDto { Success = false, Message = "Unknown error from API" };
        }

        public async Task<List<HeroImageDto>> GetHeroImagesAsync(string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/heroimages";
            var result = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, token, null, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<HeroImageDto>>(result);
            return list ?? new List<HeroImageDto>();
        }

        public async Task<bool> DeleteHeroImageAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_baseUrl}/api/heroimages/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, token, null, userAgent);
            return response?.IsSuccessStatusCode ?? false;
        }
    }
}
