using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class WineAIRepository : Repository<WineAI>, IWineAIRepository
    {
        private readonly ApplicationDbContext _db;
        public WineAIRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void AddWineAI(string question, string answer, string dispositivo)
        {
            var wine_ai = new WineAI
            {
                Dispositivo = dispositivo,
                Question = question,
                Answer = answer,
                CreationDate = DateTime.Now
            };
            _db.WineAI.Add(wine_ai);
            _db.SaveChanges();
        }

        public List<string> GetAllQuestion(DateTime data)
        {
            return _db.WineAI.Where(x => x.CreationDate == data)
                .Select(x => x.Question).ToList();
        }

        public List<string> GetAllAnswers(DateTime data)
        {
            return _db.WineAI.Where(x => x.CreationDate == data)
                .Select(x => x.Answer).ToList();
        }

    }
}