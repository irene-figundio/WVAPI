using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class EventRepository : EfRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
