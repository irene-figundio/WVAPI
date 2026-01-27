using Microsoft.EntityFrameworkCore;
using AI_Integration.DataAccess.Database.Repositories.interfaces;

namespace AI_Integration.DataAccess.Database.Repositories.impl
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
