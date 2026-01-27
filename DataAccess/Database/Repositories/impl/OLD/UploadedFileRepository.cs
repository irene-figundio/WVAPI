using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class UploadedFileRepository : Repository<UploadedFile>, IUploadedFileRepository
    {
        private readonly ApplicationDbContext _db;

        public UploadedFileRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Add(UploadedFile uploadedFile)
        {
            _db.UploadedFiles.Add(uploadedFile);
            _db.SaveChanges();
        }
        public void Update(UploadedFile uploadedFile)
        {
            _db.UploadedFiles.Update(uploadedFile);
            _db.SaveChanges();
        }

        public void Delete(UploadedFile uploadedFile)
        {
            _db.UploadedFiles.Remove(uploadedFile);
            _db.SaveChanges();
        }

        //UploadedFile IUploadedFileRepository.GetById(int id)
        //{
        //    return _db.UploadedFiles.FirstOrDefault(x => x.Id == id);

        //}

        //IEnumerable<UploadedFile> IUploadedFileRepository.GetAll()
        //{
        //    var uploadedFiles = (from file in _db.UploadedFiles
        //                         select file).ToList();
        //    return uploadedFiles;
        //}
    }
}
