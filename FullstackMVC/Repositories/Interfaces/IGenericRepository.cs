namespace FullstackMVC.Repositories.Interfaces
{
    /// <summary>
    /// Generic Repository Interface - تطبيق لمبدأ Single Responsibility
    /// يتيح عمليات CRUD الأساسية لأي Entity
    /// </summary>
    public interface IGenericRepository<T>
        where T : class
    {
        // Read Operations
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(params object[] id);

        IQueryable<T> GetQueryable();

        // Write Operations
        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        // Check Existence
        Task<bool> ExistsAsync(params object[] id);

        Task<int> CountAsync();
    }
}
