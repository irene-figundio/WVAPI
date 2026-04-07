using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class ContentRepository : EfRepository<Content>, IContentRepository
    {
        public ContentRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
