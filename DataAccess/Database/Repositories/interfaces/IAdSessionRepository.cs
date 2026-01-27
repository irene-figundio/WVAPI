using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IAdSessionRepository
    {
        List<AdSession> GetActiveSessions();
        List<AdSession> GetByCampaignId(int campaignId);
        //IEnumerable<AdSession> GetAll();
        //public List<AdSession> GetActiveSessions();

        //public void UpdateSession(AdSession session);

        //public void DeleteSession(AdSession session);

        //public void DeleteAllSessions();

        //public void DeleteAdSessionsByCampaignId(int campaignId);
        //public void DeleteSessionBySessionId(int SessionId);

        //public AdSession GetSessionByCampaignId(int CampaignId);

        //public List<AIVideo> GetVideosBySession(int SessionId);

        //public AdSession GetSessionBySessionId(int SessionId);
        //public Boolean Exists(int SessionId);

        //public AdSession GetBySessionId(int SessionId);

        //public void AddSession(AdSession session);


    }
}
