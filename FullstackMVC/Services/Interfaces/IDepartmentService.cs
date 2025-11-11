namespace FullstackMVC.Services.Interfaces
{
    using FullstackMVC.Models;

    /// <summary>
    /// Department Service Interface
    /// </summary>
    public interface IDepartmentService : IBaseService<Department>
    {
        Task<Department?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Department>> GetAllWithRelationsAsync();

        Task<bool> CanDeleteDepartmentAsync(int id);

        Task<int> GetEmployeesCountAsync(int id);
    }
}
