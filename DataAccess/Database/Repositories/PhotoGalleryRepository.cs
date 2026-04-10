using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class PhotoGalleryRepository : EfRepository<PhotoGallery>, IPhotoGalleryRepository
    {
        public PhotoGalleryRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
