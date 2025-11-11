namespace FullstackMVC.Controllers
{
    using FullstackMVC.Filters;
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize] // يتطلب تسجيل دخول لكل الـ Actions
    [GlobalExceptionFilter] // Global exception handling for all actions
    [DepartmentLocationAction] // Log all actions
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        private readonly IDepartmentService _departmentService;

        // Dependency Injection
        public CourseController(ICourseService courseService, IDepartmentService departmentService)
        {
            _courseService = courseService;
            _departmentService = departmentService;
        }

        // GET: Course/GetAll
        [CacheResult(durationSeconds: 60)] // Cache for 1 minute
        [AddCustomHeader("X-Course-List", "Accessed")]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllWithDepartmentAsync();
            return View(courses);
        }

        // GET: Course/Details/5
        [ResourceOptimization]
        [AddCustomHeader("X-Course-Details", "Viewed")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Add
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يضيفوا courses
        public async Task<IActionResult> Add()
        {
            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }

        // POST: Course/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يضيفوا courses
        public async Task<IActionResult> Add(Course course)
        {
            // Custom validation: MinDegree should be less than Degree
            if (course.MinDegree >= course.Degree)
            {
                ModelState.AddModelError("MinDegree", "Minimum degree must be less than maximum degree");
            }

            if (ModelState.IsValid)
            {
                await _courseService.CreateAsync(course);
                TempData["SuccessMessage"] = $"Course '{course.Name}' has been added successfully!";
                return RedirectToAction(nameof(GetAll));
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", course.DeptId);
            return View(course);
        }

        // GET: Course/Edit/5
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يعدلوا courses
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", course.DeptId);
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يعدلوا courses
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Num)
            {
                return NotFound();
            }

            // Custom validation: MinDegree should be less than Degree
            if (course.MinDegree >= course.Degree)
            {
                ModelState.AddModelError("MinDegree", "Minimum degree must be less than maximum degree");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseService.UpdateAsync(course);
                    TempData["SuccessMessage"] = $"Course '{course.Name}' has been updated successfully!";
                    return RedirectToAction(nameof(GetAll));
                }
                catch (Exception ex)
                {
                    if (!await _courseService.ExistsAsync(course.Num))
                    {
                        return NotFound();
                    }
                    ModelState.AddModelError("", $"Error updating course: {ex.Message}");
                }
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", course.DeptId);
            return View(course);
        }

        // GET: Course/Delete/5
        [Authorize(Roles = "Admin")] // فقط Admin يقدر يحذف courses
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // فقط Admin يقدر يحذف courses
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Course has been deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting course: {ex.Message}";
            }

            return RedirectToAction(nameof(GetAll));
        }

        // GET: Course/Students/5 - View all students enrolled in a course
        [CacheResult(durationSeconds: 120)] // Cache for 2 minutes
        public async Task<IActionResult> Students(int id)
        {
            var course = await _courseService.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Instructors/5 - View all instructors teaching a course
        [CacheResult(durationSeconds: 120)] // Cache for 2 minutes
        public async Task<IActionResult> Instructors(int id)
        {
            var course = await _courseService.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/AdminPanel/admin - Admin-only action
        [RequireAdminUrl]
        [AddCustomHeader("X-Admin-Access", "Granted")]
        public async Task<IActionResult> AdminPanel()
        {
            var departments = await _departmentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();

            var stats = new
            {
                TotalCourses = courses.Count(),
                TotalDepartments = departments.Count()
            };

            ViewBag.Stats = stats;
            return View();
        }

        // GET: Course/ManageCourses/admin - Admin course management
        [RequireAdminUrl]
        [AddCustomHeader("X-Admin-Management", "Courses")]
        public async Task<IActionResult> ManageCourses()
        {
            var courses = await _courseService.GetAllWithDepartmentAsync();
            return View(courses.OrderByDescending(c => c.Num));
        }
    }
}
