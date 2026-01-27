using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IWineAIRepository
    {
        void AddWineAI(string question, string answer, string dispositivo);
        List<string> GetAllQuestion(DateTime data);
        List<string> GetAllAnswers(DateTime data);
    }
}