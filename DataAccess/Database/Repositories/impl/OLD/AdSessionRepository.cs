using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class AdSessionRepository : Repository<AdSession>, IAdSessionRepository
    {
        private readonly ApplicationDbContext _db;

        public AdSessionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void AddSession(AdSession session)
        {
            _db.AdSession.Add(session);
            _db.SaveChanges();
        }

        public void DeleteAllSessions()
        {
            _db.AdSession.RemoveRange(_db.AdSession);
            _db.SaveChanges();
        }

        public void DeleteSession(AdSession session)
        {
            _db.AdSession.Remove(session);
            _db.SaveChanges();
        }

        public void DeleteSessionBySessionId(int SessionId)
        {
            _db.AdSession.RemoveRange(_db.AdSession.Where(x => x.Id == SessionId));
            _db.SaveChanges();
        }

        public List<AdSession> GetActiveSessions()
        {
            var activeSessions = (from session in _db.AdSession
                                              join campaign in _db.AdCampaign on session.ID_Campaing equals campaign.Id
                                              where session.EndDate < DateTime.Now && session.StartDate > DateTime.Now
                                                    && campaign.EndDate < DateTime.Now && campaign.StartDate > DateTime.Now
                                              select session).ToList();
            return activeSessions;
        }

        public IEnumerable<AdSession> GetAll()
        {
            return _db.AdSession.ToList();
        }

        public AdSession GetBySessionId(int SessionId)
        {
           return _db.AdSession.FirstOrDefault(x => x.Id == SessionId);
        }
        public Boolean Exists(int SessionId)
        {
            if (GetBySessionId(SessionId)!= null)
            {
                return true;
            }
           return false;
        }

        public AdSession GetSessionByCampaignId(int CampaignId)
        {
            return _db.AdSession.FirstOrDefault(x => x.ID_Campaing == CampaignId);
        }

        public AdSession GetSessionBySessionId(int SessionId)
        {
            return GetBySessionId(SessionId);
        }

        public List<AIVideo> GetVideosBySession(int SessionId)
        {
            return _db.AIVideo.Where(x => x.Id == SessionId).ToList();
        }

        public void UpdateSession(AdSession session)
        {
            _db.AdSession.Update(session);
            _db.SaveChanges();
        }

        public void DeleteAdSessionsByCampaignId(int campaignId)
        {
            _db.AdSession.RemoveRange(_db.AdSession.Where(x => x.ID_Campaing == campaignId));
            _db.SaveChanges();
        }
    }
}
