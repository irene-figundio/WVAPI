using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DbContext _ctx;
        public UsersRepository(DbContext ctx) { _ctx = ctx; }

        public User? GetByUsername(string username)
            => _ctx.Set<User>()
                   .Include(u => u.Status)
                   .FirstOrDefault(u => u.Username == username && !u.IsDeleted);

        public void UpdateLastLoginTime(int userId, System.DateTime loginTime)
        {
            var u = _ctx.Set<User>().FirstOrDefault(x => x.Id == userId);
            if (u != null) u.LastLoginTime = loginTime;
        }

        public bool IsActive(int statusId) => statusId == 1;

        public User? GetById(int id)
            => _ctx.Set<User>().FirstOrDefault(u => u.Id == id && !u.IsDeleted);
    }
}
