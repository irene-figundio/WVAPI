using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IContentsService
    {
        Task<List<ContentDto>> GetContentsByTypeAsync(string contentType, int langId);
        Task<bool> CreateContentAsync(CreateContentRequest request);
    }
}
