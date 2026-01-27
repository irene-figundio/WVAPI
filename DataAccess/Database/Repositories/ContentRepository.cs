using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext _db;

        public ContentRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
