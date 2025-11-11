namespace FullstackMVC.Controllers
{
    using FullstackMVC.Context;
    using FullstackMVC.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly CompanyContext _context;

        public StudentDashboardController(
            UserManager<ApplicationUser> userManager,
            CompanyContext context
        )
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /StudentDashboard/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentSSN == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var student = await _context
                .Students.Include(s => s.Department)
                .Include(s => s.Grades)
                .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(s => s.SSN == user.StudentSSN);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: /StudentDashboard/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentSSN == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var grades = await _context
                .Grades.Include(g => g.Course)
                .ThenInclude(c => c.Department)
                .Where(g => g.StudentSSN == user.StudentSSN)
                .ToListAsync();

            return View(grades);
        }

        // GET: /StudentDashboard/AllCourses
        public async Task<IActionResult> AllCourses()
        {
            var courses = await _context
                .Courses.Include(c => c.Department)
                .Include(c => c.Instructor)
                .ToListAsync();

            return View(courses);
        }

        // GET: /StudentDashboard/Departments
        public async Task<IActionResult> Departments()
        {
            var departments = await _context
                .Departments.Include(d => d.Students)
                .Include(d => d.Courses)
                .ToListAsync();

            return View(departments);
        }

        // GET: /StudentDashboard/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentSSN == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s =>
                s.SSN == user.StudentSSN
            );

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: /StudentDashboard/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Student model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.StudentSSN == null || user.StudentSSN != model.SSN)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                var student = await _context.Students.FindAsync(model.SSN);
                if (student == null)
                {
                    return NotFound();
                }

                // Only allow editing certain fields
                student.Name = model.Name;
                student.Address = model.Address;
                student.Image = model.Image;
                // Note: Cannot change SSN, Age, Gender, DeptId

                _context.Update(student);
                await _context.SaveChangesAsync();

                // Update ApplicationUser info
                user.FullName = model.Name;
                user.Address = model.Address;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
    }
}
