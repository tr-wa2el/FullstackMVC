namespace FullstackMVC.Services.Implementations
{
    using FullstackMVC.Models;
    using FullstackMVC.Repositories.Interfaces;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Student>().GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Student>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Student>> GetAllWithDepartmentAsync()
        {
            return await _unitOfWork
                .Repository<Student>()
                .GetQueryable()
                .Include(s => s.Department)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdWithDetailsAsync(int ssn)
        {
            return await _unitOfWork
                .Repository<Student>()
                .GetQueryable()
                .Include(s => s.Department)
                .Include(s => s.Grades)
                .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(s => s.SSN == ssn);
        }

        public async Task<IEnumerable<Student>> GetStudentsByDepartmentAsync(int departmentId)
        {
            return await _unitOfWork
                .Repository<Student>()
                .GetQueryable()
                .Where(s => s.DeptId == departmentId)
                .Include(s => s.Department)
                .ToListAsync();
        }

        public async Task<decimal?> GetStudentAverageGradeAsync(int ssn)
        {
            var grades = await _unitOfWork
                .Repository<Grade>()
                .GetQueryable()
                .Where(g => g.StudentSSN == ssn && g.GradeValue.HasValue)
                .ToListAsync();

            if (!grades.Any())
                return null;

            return grades.Average(g => g.GradeValue!.Value);
        }

        public async Task<Student> CreateAsync(Student entity)
        {
            await _unitOfWork.Repository<Student>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Student entity)
        {
            _unitOfWork.Repository<Student>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] id)
        {
            var student = await GetByIdAsync(id);
            if (student != null)
            {
                _unitOfWork.Repository<Student>().Delete(student);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Student>().ExistsAsync(id);
        }

        public async Task<bool> IsSSNUniqueAsync(int ssn, int? currentSSN = null)
        {
            var query = _unitOfWork.Repository<Student>().GetQueryable();

            if (currentSSN.HasValue)
            {
                query = query.Where(s => s.SSN != currentSSN.Value);
            }

            return !await query.AnyAsync(s => s.SSN == ssn);
        }
    }
}
