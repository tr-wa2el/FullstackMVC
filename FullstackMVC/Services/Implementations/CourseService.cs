namespace FullstackMVC.Services.Implementations
{
    using FullstackMVC.Models;
    using FullstackMVC.Repositories.Interfaces;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Course Service Implementation
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Course>().GetAllAsync();
        }

        public async Task<Course?> GetByIdAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Course>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Course>> GetAllWithDepartmentAsync()
        {
            return await _unitOfWork
                .Repository<Course>()
                .GetQueryable()
                .Include(c => c.Department)
                .Include(c => c.Grades)
                .Include(c => c.Instructors)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithDetailsAsync(int id)
        {
            return await _unitOfWork
                .Repository<Course>()
                .GetQueryable()
                .Include(c => c.Department)
                .Include(c => c.Grades)
                .ThenInclude(g => g.Student)
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.Num == id);
        }

        public async Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(int departmentId)
        {
            return await _unitOfWork
                .Repository<Course>()
                .GetQueryable()
                .Where(c => c.DeptId == departmentId)
                .Include(c => c.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetCourseStudentsAsync(int courseId)
        {
            return await _unitOfWork
                .Repository<Grade>()
                .GetQueryable()
                .Where(g => g.CourseNum == courseId)
                .Select(g => g.Student!)
                .Include(s => s.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Instructor>> GetCourseInstructorsAsync(int courseId)
        {
            var course = await _unitOfWork
                .Repository<Course>()
                .GetQueryable()
                .Include(c => c.Instructors)
                .ThenInclude(i => i!.Department)
                .FirstOrDefaultAsync(c => c.Num == courseId);

            return course?.Instructors ?? new List<Instructor>();
        }

        public async Task<bool> IsCourseNameUniqueAsync(string name, int? currentId = null)
        {
            var query = _unitOfWork.Repository<Course>().GetQueryable();

            if (currentId.HasValue)
            {
                query = query.Where(c => c.Num != currentId.Value);
            }

            return !await query.AnyAsync(c => c.Name == name);
        }

        public async Task<Course> CreateAsync(Course entity)
        {
            await _unitOfWork.Repository<Course>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Course entity)
        {
            _unitOfWork.Repository<Course>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] id)
        {
            var course = await GetByIdAsync(id);
            if (course != null)
            {
                _unitOfWork.Repository<Course>().Delete(course);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Course>().ExistsAsync(id);
        }
    }
}
