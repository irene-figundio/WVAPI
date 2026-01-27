using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IAdCampaignRepository
    {
        public interface IAdCampaignRepository
        {
            List<AdCampaign> GetActiveAdCampaigns();
        }
        //IEnumerable<AdCampaign> GetAll();
        //public List<AdCampaign> GetActiveAdCampaigns();

        //public AdCampaign GetByCampaignId(int campaignId);
        //public Boolean Exists(int campaignId);

        //public void AddCampaign(AdCampaign campaign);
        //public void DeleteCampaignByCampaignId(int campaignId);

        //public void UpdateCampaign(AdCampaign campaign);
        //public List<AdSession> GetAdSessionsByCampaignId(int campaignId);
        //public List<AIVideo> GetVideosByCampaignId(int campaignId);
    }
}
