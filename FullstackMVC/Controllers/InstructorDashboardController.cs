namespace FullstackMVC.Controllers
{
    using FullstackMVC.Context;
    using FullstackMVC.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly CompanyContext _context;

        public InstructorDashboardController(
            UserManager<ApplicationUser> userManager,
            CompanyContext context
        )
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /InstructorDashboard/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var instructor = await _context
                .Instructors.Include(i => i.Department)
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Id == user.InstructorId);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: /InstructorDashboard/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var courses = await _context
                .Courses.Include(c => c.Department)
                .Where(c => c.InstructorId == user.InstructorId)
                .ToListAsync();

            return View(courses);
        }

        // GET: /InstructorDashboard/CourseStudents
        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Verify the course belongs to this instructor
            var course = await _context
                .Courses.Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Num == courseId && c.InstructorId == user.InstructorId);

            if (course == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var grades = await _context
                .Grades.Include(g => g.Student)
                .Where(g => g.CourseNum == courseId)
                .ToListAsync();

            ViewBag.Course = course;
            return View(grades);
        }

        // GET: /InstructorDashboard/AddGrade
        public async Task<IActionResult> AddGrade(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Verify the course belongs to this instructor
            var course = await _context.Courses.FirstOrDefaultAsync(c =>
                c.Num == courseId && c.InstructorId == user.InstructorId
            );

            if (course == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Get students who don't have a grade for this course yet
            var enrolledStudentSSNs = await _context
                .Grades.Where(g => g.CourseNum == courseId)
                .Select(g => g.StudentSSN)
                .ToListAsync();

            var availableStudents = await _context
                .Students.Where(s => !enrolledStudentSSNs.Contains(s.SSN))
                .ToListAsync();

            ViewBag.CourseId = courseId;
            ViewBag.CourseName = course.Name;
            ViewBag.Students = new SelectList(availableStudents, "SSN", "Name");
            ViewBag.MaxDegree = course.Degree;
            ViewBag.MinDegree = course.MinDegree;

            return View();
        }

        // POST: /InstructorDashboard/AddGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(Grade model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Verify the course belongs to this instructor
            var course = await _context.Courses.FirstOrDefaultAsync(c =>
                c.Num == model.CourseNum && c.InstructorId == user.InstructorId
            );

            if (course == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Validate grade value
            if (model.GradeValue < 0 || model.GradeValue > course.Degree)
            {
                ModelState.AddModelError(
                    "GradeValue",
                    $"Grade must be between 0 and {course.Degree}"
                );
            }

            if (ModelState.IsValid)
            {
                _context.Grades.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Grade added successfully!";
                return RedirectToAction("CourseStudents", new { courseId = model.CourseNum });
            }

            // Reload students for dropdown
            var enrolledStudentSSNs = await _context
                .Grades.Where(g => g.CourseNum == model.CourseNum)
                .Select(g => g.StudentSSN)
                .ToListAsync();

            var availableStudents = await _context
                .Students.Where(s => !enrolledStudentSSNs.Contains(s.SSN))
                .ToListAsync();

            ViewBag.CourseId = model.CourseNum;
            ViewBag.CourseName = course.Name;
            ViewBag.Students = new SelectList(availableStudents, "SSN", "Name");
            ViewBag.MaxDegree = course.Degree;
            ViewBag.MinDegree = course.MinDegree;

            return View(model);
        }

        // GET: /InstructorDashboard/EditGrade
        public async Task<IActionResult> EditGrade(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var grade = await _context
                .Grades.Include(g => g.Course)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null || grade.Course.InstructorId != user.InstructorId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.MaxDegree = grade.Course.Degree;
            ViewBag.MinDegree = grade.Course.MinDegree;

            return View(grade);
        }

        // POST: /InstructorDashboard/EditGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(Grade model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var grade = await _context
                .Grades.Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == model.Id);

            if (grade == null || grade.Course.InstructorId != user.InstructorId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Validate grade value
            if (model.GradeValue < 0 || model.GradeValue > grade.Course.Degree)
            {
                ModelState.AddModelError(
                    "GradeValue",
                    $"Grade must be between 0 and {grade.Course.Degree}"
                );
            }

            if (ModelState.IsValid)
            {
                grade.GradeValue = model.GradeValue;
                _context.Update(grade);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Grade updated successfully!";
                return RedirectToAction("CourseStudents", new { courseId = grade.CourseNum });
            }

            ViewBag.MaxDegree = grade.Course.Degree;
            ViewBag.MinDegree = grade.Course.MinDegree;

            return View(model);
        }

        // GET: /InstructorDashboard/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i =>
                i.Id == user.InstructorId
            );

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: /InstructorDashboard/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Instructor model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.InstructorId == null || user.InstructorId != model.Id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                var instructor = await _context.Instructors.FindAsync(model.Id);
                if (instructor == null)
                {
                    return NotFound();
                }

                // Only allow editing certain fields (NOT salary, dept, courses)
                instructor.Name = model.Name;
                instructor.Address = model.Address;
                instructor.Email = model.Email;
                instructor.Age = model.Age;
                instructor.Degree = model.Degree;

                _context.Update(instructor);
                await _context.SaveChangesAsync();

                // Update ApplicationUser info
                user.FullName = model.Name;
                user.Address = model.Address;
                user.Email = model.Email;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
    }
}
