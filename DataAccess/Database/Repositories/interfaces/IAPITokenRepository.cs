using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IAPITokenRepository
    {
        APIToken? GetAPIToken(string token);
        //void AddToken(APIToken token);
        // IEnumerable<APIToken> GetAll();
    }
}
