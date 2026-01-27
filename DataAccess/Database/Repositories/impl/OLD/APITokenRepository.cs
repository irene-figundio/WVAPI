using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class APITokenRepository : Repository<APIToken>,  IAPITokenRepository
    {
        private readonly ApplicationDbContext _db;

        public APITokenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public APIToken GetAPIToken(string token)
        {
            return _db.APIToken.FirstOrDefault(t => t.Token == token);
        }

        public void AddToken(APIToken token)
        {
            _db.APIToken.Add(token);
            _db.SaveChanges();
        }

        public IEnumerable<APIToken> GetAll()
        {
            return _db.APIToken.ToList();
        }
    }
}
