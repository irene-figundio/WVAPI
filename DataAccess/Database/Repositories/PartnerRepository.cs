using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly ApplicationDbContext _db;

        public PartnerRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
