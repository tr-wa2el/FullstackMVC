namespace FullstackMVC.Services.Interfaces
{
    /// <summary>
    /// Base Service Interface - تطبيق لمبدأ Interface Segregation
    /// يوفر العمليات الأساسية لكل Service
    /// </summary>
    public interface IBaseService<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(params object[] id);

        Task<T> CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(params object[] id);

        Task<bool> ExistsAsync(params object[] id);
    }
}
