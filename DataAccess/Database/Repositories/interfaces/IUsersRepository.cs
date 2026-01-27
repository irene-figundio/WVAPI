using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IUsersRepository
    {
        User? GetByUsername(string username);
        void UpdateLastLoginTime(int userId, DateTime loginTime);
        bool IsActive(int statusId); // statusId == 1 => Active

        //IQueryable<User> Queryable();      // per listing/paging
        //User? GetById(int id);
        //void Insert(User user);
        //void Update(User user);
    }
}
