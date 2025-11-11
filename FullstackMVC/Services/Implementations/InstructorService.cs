namespace FullstackMVC.Services.Implementations
{
    using FullstackMVC.Models;
    using FullstackMVC.Repositories.Interfaces;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Instructor Service Implementation
    /// </summary>
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Instructor>().GetAllAsync();
        }

        public async Task<Instructor?> GetByIdAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Instructor>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Instructor>> GetAllWithDepartmentAsync()
        {
            return await _unitOfWork
                .Repository<Instructor>()
                .GetQueryable()
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .ToListAsync();
        }

        public async Task<Instructor?> GetByIdWithDetailsAsync(int id)
        {
            return await _unitOfWork
                .Repository<Instructor>()
                .GetQueryable()
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Instructor>> GetInstructorsByDepartmentAsync(int departmentId)
        {
            return await _unitOfWork
                .Repository<Instructor>()
                .GetQueryable()
                .Where(i => i.DeptId == departmentId)
                .Include(i => i.Department)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? currentId = null)
        {
            var query = _unitOfWork.Repository<Instructor>().GetQueryable();

            if (currentId.HasValue)
            {
                query = query.Where(i => i.Id != currentId.Value);
            }

            return !await query.AnyAsync(i => i.Email == email);
        }

        public async Task<Instructor> CreateAsync(Instructor entity)
        {
            await _unitOfWork.Repository<Instructor>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Instructor entity)
        {
            _unitOfWork.Repository<Instructor>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] id)
        {
            var instructor = await GetByIdAsync(id);
            if (instructor != null)
            {
                _unitOfWork.Repository<Instructor>().Delete(instructor);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(params object[] id)
        {
            return await _unitOfWork.Repository<Instructor>().ExistsAsync(id);
        }
    }
}
