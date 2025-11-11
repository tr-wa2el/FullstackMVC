namespace FullstackMVC.Services.Interfaces
{
    using FullstackMVC.Models;

    /// <summary>
    /// Course Service Interface
    /// </summary>
    public interface ICourseService : IBaseService<Course>
    {
        Task<Course?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Course>> GetAllWithDepartmentAsync();

        Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(int departmentId);

        Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId);

        Task<IEnumerable<Instructor>> GetCourseInstructorsAsync(int courseId);

        Task<bool> IsCourseNameUniqueAsync(string name, int? currentId = null);
    }
}
