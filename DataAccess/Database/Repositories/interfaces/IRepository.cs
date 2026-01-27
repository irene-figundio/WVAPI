using System.Linq.Expressions;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IRepository<T> where T : class
    {
        //T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        //IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        //void Add(T entity);
        //bool Exists(Expression<Func<T, bool>> filter);
        //void Delete(T entity, int? userId = null);

        IQueryable<T> Query(bool asNoTracking = true);
        Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
        Task InsertAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
    }
}
