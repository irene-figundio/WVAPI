using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class AdCampaignRepository : IAdCampaignRepository
    {
        private readonly ApplicationDbContext _db;

        public AdCampaignRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<AdCampaign> GetActiveAdCampaigns()
        {
            var now = DateTime.Now;
            return _db.Set<AdCampaign>()
                       .Where(c => c.StartDate <= now && c.EndDate >= now && (c.IsDeleted == null || c.IsDeleted == false))
                       .ToList();
        }
        public List<AIVideo> GetVideosByCampaignId(int campaignId)
        {
            var videos = (from video in _db.AIVideo
                          join session in _db.AdSession on video.ID_Session equals session.Id
                          join campaign in _db.AdCampaign on session.ID_Campaing equals campaign.Id
                          where campaign.Id == campaignId
                          select video).ToList();
            return videos;
        }

    }
}
