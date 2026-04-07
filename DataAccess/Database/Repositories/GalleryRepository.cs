using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class GalleryRepository : EfRepository<Gallery>, IGalleryRepository
    {
        public GalleryRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
