using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IAdAnalyticsRepository
    {
        List<AdAnalytics> GetByDate(DateTime date);
        int GetClicksCountByDateAndSessionId(DateTime date, int sessionId);
        //public void AddAdAnalytics(AdAnalytics adAnalytics);
        //public void RemoveAdAnalytics(AdAnalytics adAnalytics);
        //public AdAnalytics GetAdAnalyticsById(int id);
        //public List<AdAnalytics> GetAllAdAnalytics();
        //public void UpdateAdAnalytics(AdAnalytics adAnalytics);
        //public List<AdAnalytics> GetAdAnalyticsByVideoId(int id);

        //public List<AdAnalytics> GetAdAnalyticsByDate(DateTime date);
        //public List<AdAnalytics> GetAdAnalyticsByDateAndVideoId(DateTime date, int videoId);
        //public List<AdAnalytics> GetAdAnalyticsByDateAndSessionId(DateTime date, int sessionId);
        //public List<AdAnalytics> GetAdAnalyticsBySessionId(int sessionId);
        //public List<AdAnalytics> GetAdAnalyticsByVideoIdAndSessionId(int videoId, int sessionId);
        //public int GetViewsCountByDateAndVideoId(DateTime date, int videoId);

        //public int GetClicksCountByDateAndVideoId(DateTime date, int videoId);
        //public int GetViewsCountByDateAndSessionId(DateTime date, int sessionId);
        //public int GetClicksCountByDateAndSessionId(DateTime date, int sessionId);
    }
}
