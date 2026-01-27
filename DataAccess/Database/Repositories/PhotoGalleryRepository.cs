using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class PhotoGalleryRepository : IPhotoGalleryRepository
    {
        private readonly ApplicationDbContext _db;

        public PhotoGalleryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
