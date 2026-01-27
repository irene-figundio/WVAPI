using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class ContentLinkRepository : IContentLinkRepository
    {
        private readonly ApplicationDbContext _db;

        public ContentLinkRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
