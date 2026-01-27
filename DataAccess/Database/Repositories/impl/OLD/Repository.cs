using Abp.Domain.Repositories;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AI_Integration.DataAccess.Database.Repositories.impl
{
    public class Repository<T> : interfaces.IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }


            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            return query.Any(filter);
        }

        public void Delete(T entity, int? userId = null)
        {
            var isDeletedProp = entity.GetType().GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);
            var deletionTimeProp = entity.GetType().GetProperty("DeletionTime", BindingFlags.Public | BindingFlags.Instance);
            var deletionUserIdProp = entity.GetType().GetProperty("DeletionUserId", BindingFlags.Public | BindingFlags.Instance);

            if (isDeletedProp != null)
            {
                isDeletedProp.SetValue(entity, true, null);
            }

            if (deletionTimeProp != null)
            {
                deletionTimeProp.SetValue(entity, DateTime.Now, null);
            }

            if (deletionUserIdProp != null && userId.HasValue)
            {
                deletionUserIdProp.SetValue(entity, userId.Value, null);
            }
        }
    }
}
