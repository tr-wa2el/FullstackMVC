namespace FullstackMVC.Services.Interfaces
{
    using FullstackMVC.Models;

    /// <summary>
    /// Instructor Service Interface
    /// </summary>
    public interface IInstructorService : IBaseService<Instructor>
    {
        Task<Instructor?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Instructor>> GetAllWithDepartmentAsync();

        Task<IEnumerable<Instructor>> GetInstructorsByDepartmentAsync(int departmentId);

        Task<bool> IsEmailUniqueAsync(string email, int? currentId = null);
    }
}
