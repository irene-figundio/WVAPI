using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IAiVideoService
    {
        Task<PagedResult<VideoListItemDto>> GetVideosAsync(string? query, int page);
        Task<bool> CreateVideoAsync(CreateVideoRequest request);
    }
}
