using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IAiVideoService
    {
        Task<PagedResult<VideoListItemDto>> GetVideosAsync(string? query, int page, string sessionToken,string userAgent, CancellationToken ct);
        Task<bool> CreateVideoAsync(CreateVideoRequest request, string sessionToken, string userAgent, CancellationToken ct);
        Task<bool> DeleteVideoAsync(int id, string sessionToken, string userAgent, CancellationToken ct);
        Task<bool> UpdateVideoAsync(int id, UpdateVideoRequest request, string sessionToken, string userAgent, CancellationToken ct);
    }
}
