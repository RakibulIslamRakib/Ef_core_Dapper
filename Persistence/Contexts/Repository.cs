using Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Contexts
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {

            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry => entry.State = EntityState.Unchanged);
            }

            _context.SaveChanges();
            return exception.ToString();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await _dbSet.AddAsync(entity);
                return await SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task<int> InsertAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                await _dbSet.AddRangeAsync(entities);
               return  await SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {

                throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _dbSet.Update(entity);
                await SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                _dbSet.UpdateRange(entities);
                await SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {

                throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task DeleteAsync(object id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var entity = await GetByIdAsync(id);
            if (entity != null)
            {             
                try
                {
                    _dbSet.Remove(entity);
                    await SaveChangesAsync();
                }
                catch (DbUpdateException exception)
                {

                    throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
                }
            }             
        }

        public  void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            try
            {
                _dbSet.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new DbUpdateException(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                return _dbSet; 
            }
            return  _dbSet.Where(predicate);
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
