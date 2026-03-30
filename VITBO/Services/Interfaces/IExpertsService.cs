using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IExpertsService
    {
        Task<List<ExpertDto>> GetExpertsAsync(int langId, string token, string userAgent);
        Task<ExpertDto?> GetExpertByIdAsync(int id, int langId, string token, string userAgent);
        Task<bool> CreateExpertAsync(ExpertDto expert, string token, string userAgent);
        Task<bool> UpdateExpertAsync(int id, ExpertDto expert, string token, string userAgent);
        Task<bool> DeleteExpertAsync(int id, string token, string userAgent);
    }
}
