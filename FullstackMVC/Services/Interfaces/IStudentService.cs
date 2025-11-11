namespace FullstackMVC.Services.Interfaces
{
    using FullstackMVC.Models;

    /// <summary>
    /// Student Service Interface - تطبيق لمبدأ Dependency Inversion
    /// يفصل الـ Business Logic عن الـ Data Access
    /// </summary>
    public interface IStudentService : IBaseService<Student>
    {
        Task<IEnumerable<Student>> GetAllWithDepartmentAsync();

        Task<Student?> GetByIdWithDetailsAsync(int ssn);

        Task<IEnumerable<Student>> GetStudentsByDepartmentAsync(int departmentId);

        Task<decimal?> GetStudentAverageGradeAsync(int ssn);

        Task<bool> IsSSNUniqueAsync(int ssn, int? currentSSN = null);
    }
}
