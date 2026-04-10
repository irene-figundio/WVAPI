using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class ContentImageRepository : EfRepository<ContentImage>, IContentImageRepository
    {
        public ContentImageRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
