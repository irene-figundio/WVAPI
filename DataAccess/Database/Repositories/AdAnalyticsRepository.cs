using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model;
using System.Linq;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class AdAnalyticsRepository : IAdAnalyticsRepository
    {
        private readonly ApplicationDbContext _db;

        public AdAnalyticsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<AdAnalytics> GetByDate(DateTime date)
        {
            return _db.Set<AdAnalytics>()
                       .Where(a => a.SendDate.Date == date.Date )
                       // && (a.IsDeleted == null || a.IsDeleted == false))
                       .ToList();
        }

        public int GetClicksCountByDateAndSessionId(DateTime date, int sessionId)
        {
            return _db.Set<AdAnalytics>()
                       .Where(a => a.SessionId == sessionId
                                && a.SendDate.Date == date.Date )
                             //   && (a.IsDeleted == null || a.IsDeleted == false))
                       .Sum(a => a.NumClick ?? 0);
        }
        //public void AddAdAnalytics(AdAnalytics adAnalytics)
        //{
        //    _db.AdAnalytics.Add(adAnalytics);
        //    _db.SaveChanges();
        //}

        //public List<AdAnalytics> GetAdAnalyticsByDate(DateTime date)
        //{
        //    return _db.AdAnalytics.Where(x => x.SendDate == date).ToList();
        //}

        //public List<AdAnalytics> GetAdAnalyticsByDateAndSessionId(DateTime date, int sessionId)
        //{
        //    return _db.AdAnalytics.Where(x => x.SessionId == sessionId && x.SendDate == date).ToList();
        //}

        //public List<AdAnalytics> GetAdAnalyticsByDateAndVideoId(DateTime date, int videoId)
        //{
        //    return _db.AdAnalytics.Where(x => x.VideoId == videoId && x.SendDate == date).ToList();
        //}

        //public AdAnalytics GetAdAnalyticsById(int id)
        //{
        //    return _db.AdAnalytics.FirstOrDefault(x => x.Id == id);
        //}

        //public List<AdAnalytics> GetAdAnalyticsBySessionId(int sessionId)
        //{
        //    return _db.AdAnalytics.Where(x => x.SessionId == sessionId).ToList();
        //}

        //public List<AdAnalytics> GetAdAnalyticsByVideoId(int id)
        //{
        //    return _db.AdAnalytics.Where(x => x.VideoId == id).ToList();
        //}

        //public List<AdAnalytics> GetAdAnalyticsByVideoIdAndSessionId(int videoId, int sessionId)
        //{
        //    return _db.AdAnalytics.Where(x => x.VideoId == videoId && x.SessionId == sessionId).ToList();
        //}

        //public List<AdAnalytics> GetAllAdAnalytics()
        //{
        //    return _db.AdAnalytics.ToList();
        //}

        //public int GetClicksCountByDateAndSessionId(DateTime date, int sessionId)
        //{
        //    int totalNumClick = _db.AdAnalytics
        //                     .Where(x => x.SessionId == sessionId && x.SendDate == date)
        //                     .Sum(x => x.NumClick ?? 0);
        //    return totalNumClick;
        //}

        //public int GetClicksCountByDateAndVideoId(DateTime date, int videoId)
        //{
        //    int totalNumClick = _db.AdAnalytics
        //                    .Where(x => x.VideoId == videoId && x.SendDate == date)
        //                    .Sum(x => x.NumClick ?? 0);
        //    return totalNumClick;
        //}

        //public int GetViewsCountByDateAndSessionId(DateTime date, int sessionId)
        //{
        //    int totalViews = _db.AdAnalytics
        //                    .Where(x => x.SessionId == sessionId && x.SendDate == date)
        //                    .Sum(x => x.NumViews ?? 0);
        //    return totalViews;
        //}

        //public int GetViewsCountByDateAndVideoId(DateTime date, int videoId)
        //{
        //    int totalViews = _db.AdAnalytics
        //                    .Where(x => x.VideoId == videoId && x.SendDate == date)
        //                    .Sum(x => x.NumViews ?? 0);
        //    return totalViews;

        //}

        //public void RemoveAdAnalytics(AdAnalytics adAnalytics)
        //{
        //    _db.AdAnalytics.Remove(adAnalytics);
        //    _db.SaveChanges();
        //}

        //public void UpdateAdAnalytics(AdAnalytics adAnalytics)
        //{
        //    _db.AdAnalytics.Update(adAnalytics);
        //    _db.SaveChanges();
        //}


    }
}
