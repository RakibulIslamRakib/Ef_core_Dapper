using System.Linq.Expressions;

namespace Persistence.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(object id);
        Task<int> InsertAsync(T entity);
        Task<int> InsertAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);
        Task DeleteAsync(object id);
        void Delete(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null);
    }
}
