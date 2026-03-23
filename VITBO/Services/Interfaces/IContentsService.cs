using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IContentsService
    {
        Task<List<ContentDto>> GetContentsByTypeAsync(string contentType, int langId, string token, string userAgent);
        Task<bool> CreateContent(CreateContentRequest request, string token, string userAgent);
        Task<ContentDto?> GetContentByIdAsync(int id, int langId, string token, string userAgent);
        Task<bool> UpdateContentAsync(int id, ContentDto request, string token, string userAgent);
    }
}
