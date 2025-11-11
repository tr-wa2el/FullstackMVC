namespace FullstackMVC.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class;

        // Transaction Management
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
