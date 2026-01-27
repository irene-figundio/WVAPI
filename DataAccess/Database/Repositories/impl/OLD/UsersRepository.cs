using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users
                .Include(u => u.Status)
                .FirstOrDefault(u => u.Username == username && !u.IsDeleted);
        }

        public void UpdateLastLoginTime(int userId, DateTime loginTime)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LastLoginTime = loginTime;
                _context.SaveChanges();
            }
        }

        public bool IsActive(int statusId)
        {
            // 0 = NotActive, 1 = Active (dalla tabella UserStatuses)
            return statusId == 1;
        }
    }
}
