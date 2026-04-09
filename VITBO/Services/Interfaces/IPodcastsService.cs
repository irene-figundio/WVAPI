using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IPodcastsService
    {
        Task<List<PodcastDto>> GetPodcastsAsync(int langId, string token, string userAgent);
        Task<int?> CreatePodcastAsync(CreatePodcastRequest request, string token, string userAgent);
        Task<PodcastDto?> GetPodcastByIdAsync(int id, int langId, string token, string userAgent);
        Task<bool> UpdatePodcastAsync(int id, PodcastDto request, string token, string userAgent);
        Task<bool> DeletePodcastAsync(int id, string token, string userAgent);
    }
}
