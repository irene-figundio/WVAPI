using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class AdCampaignRepository : Repository<AdCampaign>, IAdCampaignRepository
    {
        private readonly ApplicationDbContext _db;

        public AdCampaignRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void AddCampaign(AdCampaign campaign)
        {
            _db.AdCampaign.Add(campaign);
            _db.SaveChanges();
        }

        public List<AdCampaign> GetActiveAdCampaigns()
        {
            return _db.AdCampaign.Where(x => x.EndDate < DateTime.Now && x.StartDate > DateTime.Now).ToList();
        }

        public IEnumerable<AdCampaign> GetAll()
        {
            return _db.AdCampaign.ToList();
        }

        public AdCampaign GetByCampaignId(int campaignId)
        {
            return _db.AdCampaign.FirstOrDefault(x => x.Id == campaignId);
        }

        public Boolean Exists(int campaignId)
        {
            if (GetByCampaignId(campaignId) != null)
            {
                return true;
            }
            return false;
        }

        public void DeleteCampaignByCampaignId(int campaignId)
        {
            _db.AdCampaign.RemoveRange(_db.AdCampaign.Where(x => x.Id == campaignId));
            _db.SaveChanges();
        }

        public void UpdateCampaign(AdCampaign campaign)
        {
            _db.AdCampaign.Update(campaign);
            _db.SaveChanges();
        }

        public List<AdSession> GetAdSessionsByCampaignId(int campaignId)
        {
            return _db.AdSession.Where(x => x.ID_Campaing == campaignId).ToList();
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
