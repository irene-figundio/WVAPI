using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class EventLinkRepository : IEventLinkRepository
    {
        private readonly ApplicationDbContext _db;

        public EventLinkRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
