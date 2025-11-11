namespace FullstackMVC.Services.Implementations
{
    using FullstackMVC.Models;
    using FullstackMVC.Repositories.Interfaces;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Department Service Implementation
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Department>().GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Department>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Department>> GetAllWithRelationsAsync()
        {
            return await _unitOfWork
                .Repository<Department>()
                .GetQueryable()
                .Include(d => d.Employees)
                .Include(d => d.Students)
                .Include(d => d.Instructors)
                .Include(d => d.Courses)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdWithDetailsAsync(int id)
        {
            return await _unitOfWork
                .Repository<Department>()
                .GetQueryable()
                .Include(d => d.Employees)
                .Include(d => d.Students)
                .Include(d => d.Instructors)
                .Include(d => d.Courses)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CanDeleteDepartmentAsync(int id)
        {
            var hasEmployees = await _unitOfWork
                .Repository<Employee>()
                .GetQueryable()
                .AnyAsync(e => e.DeptId == id);

            var hasStudents = await _unitOfWork
                .Repository<Student>()
                .GetQueryable()
                .AnyAsync(s => s.DeptId == id);

            var hasInstructors = await _unitOfWork
                .Repository<Instructor>()
                .GetQueryable()
                .AnyAsync(i => i.DeptId == id);

            var hasCourses = await _unitOfWork
                .Repository<Course>()
                .GetQueryable()
                .AnyAsync(c => c.DeptId == id);

            return !(hasEmployees || hasStudents || hasInstructors || hasCourses);
        }

        public async Task<int> GetEmployeesCountAsync(int id)
        {
            return await _unitOfWork
                .Repository<Employee>()
                .GetQueryable()
                .CountAsync(e => e.DeptId == id);
        }

        public async Task<Department> CreateAsync(Department entity)
        {
            await _unitOfWork.Repository<Department>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Department entity)
        {
            _unitOfWork.Repository<Department>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] id)
        {
            var department = await GetByIdAsync(id);
            if (department != null)
            {
                _unitOfWork.Repository<Department>().Delete(department);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Department>().ExistsAsync(id);
        }
    }
}
