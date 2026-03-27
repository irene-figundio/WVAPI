using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IMediaService
    {
        Task<List<ContentImageDto>> GetContentImagesAsync(string token, string userAgent);
        Task<List<ContentImageDto>> GetContentImagesByContentIdAsync(int? contentId, string token, string userAgent);

        Task<bool> CreateContentImageAsync(ContentImageDto model, string token, string userAgent);
        Task<bool> DeleteContentImageAsync(int id, string token, string userAgent);

        Task<List<GalleryDto>> GetGalleriesAsync(string token, string userAgent);
        Task<GalleryDto> GetGalleriesByEventIdAsync(int? eventId, string token, string userAgent);
        Task<bool> CreateGalleryAsync(GalleryDto model, string token, string userAgent);
        Task<bool> DeleteGalleryAsync(int id, string token, string userAgent);

        Task<List<PhotoGalleryDto>> GetPhotosAsync(string token, string userAgent);
        Task<List<PhotoGalleryDto>> GetPhotosByGalleryIdAsync(int? galleryId, string token, string userAgent);
        Task<bool> CreatePhotoAsync(PhotoGalleryDto model, string token, string userAgent);
        Task<bool> DeletePhotoAsync(int id, string token, string userAgent);

        Task<List<ContentExpertDto>> GetContentExpertsAsync(int contentId, string token, string userAgent);
        Task<bool> AddContentExpertAsync(ContentExpertDto model, string token, string userAgent);
        Task<bool> RemoveContentExpertAsync(int id, string token, string userAgent);

        Task<List<EventExpertDto>> GetEventExpertsAsync(int eventId, string token, string userAgent);
        Task<bool> AddEventExpertAsync(EventExpertDto model, string token, string userAgent);
        Task<bool> RemoveEventExpertAsync(int id, string token, string userAgent);

        Task<List<ContentLinkDto>> GetContentLinksAsync(int? contentId, string token, string userAgent);
        Task<bool> CreateContentLinkAsync(ContentLinkDto model, string token, string userAgent);
        Task<bool> DeleteContentLinkAsync(int id, string token, string userAgent);
    }
}
