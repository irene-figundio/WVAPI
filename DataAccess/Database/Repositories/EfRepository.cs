using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _ctx;
        private readonly DbSet<T> _set;

        public EfRepository(DbContext ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<T>();
        }
        public void Add(T entity)
        {
            _set.Add(entity);
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _set;

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
            IQueryable<T> query = _set;

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
            IQueryable<T> query = _set;

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
        public IQueryable<T> Query(bool asNoTracking = true)
            => asNoTracking ? _set.AsNoTracking() : _set.AsQueryable();

        public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
            => await _set.FindAsync(new [] { id }, ct);

        public async Task InsertAsync(T entity, CancellationToken ct = default)
            => await _set.AddAsync(entity, ct);

        public void Update(T entity) => _set.Update(entity);
        public void Remove(T entity) => _set.Remove(entity);
    }
}
