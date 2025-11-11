namespace FullstackMVC.Repositories.Implementations
{
    using FullstackMVC.Context;
    using FullstackMVC.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Generic Repository Implementation
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly CompanyContext _context;

        protected readonly DbSet<T> _dbSet;

        public GenericRepository(CompanyContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Read Operations
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(params object[] id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        // Write Operations
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // Check Existence
        public virtual async Task<bool> ExistsAsync(params object[] id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
