using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class WebAPILogRepository :IWebAPILogRepository
    {
        private readonly ApplicationDbContext _db;

        public WebAPILogRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        //public void AddLog(WebAPILog log)
        //{
        //    _db.WebAPILog.Add(log);
        //    _db.SaveChanges();
        //}

        //public IEnumerable<WebAPILog> GetAll()
        //{
        //    return _db.WebAPILog.ToList();
        //}
    }
}
