using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class ContentImageRepository : IContentImageRepository
    {
        private readonly ApplicationDbContext _db;

        public ContentImageRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
