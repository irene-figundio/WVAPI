using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class PodcastRepository : IPodcastRepository
    {
        private readonly ApplicationDbContext _db;

        public PodcastRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
