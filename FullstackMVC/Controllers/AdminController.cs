namespace FullstackMVC.Controllers
{
    using FullstackMVC.Context;
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using FullstackMVC.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly CompanyContext _context;

        private readonly IStudentService _studentService;

        private readonly IInstructorService _instructorService;

        private readonly ICourseService _courseService;

        private readonly IDepartmentService _departmentService;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            CompanyContext context,
            IStudentService studentService,
            IInstructorService instructorService,
            ICourseService courseService,
            IDepartmentService departmentService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _studentService = studentService;
            _instructorService = instructorService;
            _courseService = courseService;
            _departmentService = departmentService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalInstructors = await _context.Instructors.CountAsync();
            ViewBag.TotalCourses = await _context.Courses.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();

            return View();
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // GET: /Admin/AssignRole?userId=xxx
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewBag.User = user;
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;

            return View();
        }

        // POST: /Admin/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError("", "Role does not exist");
                return RedirectToAction("AssignRole", new { userId });
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction("Users");
        }

        // POST: /Admin/RemoveRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }

            return RedirectToAction("Users");
        }

        // === STUDENT MANAGEMENT ===

        // GET: /Admin/Students
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students.Include(s => s.Department).ToListAsync();
            return View(students);
        }

        // GET: /Admin/CreateStudent
        public async Task<IActionResult> CreateStudent()
        {
            try
            {
                ViewBag.Departments = new SelectList(
                    await _context.Departments.ToListAsync(),
                    "Id",
                    "Name"
                );
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Handle the case where database hasn't been updated yet
                ViewBag.Departments = new SelectList(new List<object>(), "Id", "Name");
                TempData["ErrorMessage"] = "Database schema needs to be updated. Please contact administrator.";
            }

            return View();
        }

        // POST: /Admin/CreateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(Student model)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student created successfully!";
                return RedirectToAction("Students");
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name"
            );
            return View(model);
        }

        // GET: /Admin/EditStudent
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                student.DeptId
            );
            return View(student);
        }

        // POST: /Admin/EditStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(Student model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student updated successfully!";
                return RedirectToAction("Students");
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                model.DeptId
            );
            return View(model);
        }

        // GET: /Admin/DeleteStudent
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context
                .Students.Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SSN == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: /Admin/DeleteStudent
        [HttpPost, ActionName("DeleteStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }

            return RedirectToAction("Students");
        }

        // === INSTRUCTOR MANAGEMENT ===

        // GET: /Admin/Instructors
        public async Task<IActionResult> Instructors()
        {
            var instructors = await _context.Instructors.Include(i => i.Department).ToListAsync();
            return View(instructors);
        }

        // GET: /Admin/CreateInstructor
        public async Task<IActionResult> CreateInstructor()
        {
            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name"
            );
            return View();
        }

        // POST: /Admin/CreateInstructor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstructor(Instructor model)
        {
            if (ModelState.IsValid)
            {
                _context.Instructors.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor created successfully!";
                return RedirectToAction("Instructors");
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name"
            );
            return View(model);
        }

        // GET: /Admin/EditInstructor
        public async Task<IActionResult> EditInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                instructor.DeptId
            );
            return View(instructor);
        }

        // POST: /Admin/EditInstructor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstructor(Instructor model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check for unique email (excluding current instructor)
                    var existingInstructor = await _context.Instructors
                        .FirstOrDefaultAsync(i => i.Email == model.Email && i.Id != model.Id);

                    if (existingInstructor != null)
                    {
                        ModelState.AddModelError("Email", "This email address is already in use.");
                        ViewBag.Departments = new SelectList(
                            await _context.Departments.ToListAsync(),
                            "Id",
                            "Name",
                            model.DeptId
                        );
                        return View(model);
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instructor updated successfully!";
                    return RedirectToAction("Instructors");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating instructor: {ex.Message}";
                    return RedirectToAction("EditInstructor", new { id = model.Id });
                }
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                model.DeptId
            );
            return View(model);
        }

        // GET: /Admin/DeleteInstructor
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context
                .Instructors.Include(i => i.Department)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: /Admin/DeleteInstructor
        [HttpPost, ActionName("DeleteInstructor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructorConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor deleted successfully!";
            }

            return RedirectToAction("Instructors");
        }

        // === COURSE MANAGEMENT ===

        // GET: /Admin/Courses
        public async Task<IActionResult> Courses()
        {
            var courses = await _context
                .Courses.Include(c => c.Department)
                .Include(c => c.Instructor)
                .ToListAsync();
            return View(courses);
        }

        // GET: /Admin/CreateCourse
        public async Task<IActionResult> CreateCourse()
        {
            try
            {
                ViewBag.Departments = new SelectList(
                    await _context.Departments.ToListAsync(),
                    "Id",
                    "Name"
                );
                ViewBag.Instructors = new SelectList(
                    await _context.Instructors.ToListAsync(),
                    "Id",
                    "Name"
                );
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Handle the case where database hasn't been updated yet
                ViewBag.Departments = new SelectList(new List<object>(), "Id", "Name");
                ViewBag.Instructors = new SelectList(new List<object>(), "Id", "Name");
                TempData["ErrorMessage"] = "Database schema needs to be updated. Please run the SQL fix script.";
            }
            return View();
        }

        // POST: /Admin/CreateCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction("Courses");
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name"
            );
            ViewBag.Instructors = new SelectList(
                await _context.Instructors.ToListAsync(),
                "Id",
                "Name"
            );
            return View(model);
        }

        // GET: /Admin/EditCourse
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                course.DeptId
            );
            ViewBag.Instructors = new SelectList(
                await _context.Instructors.ToListAsync(),
                "Id",
                "Name",
                course.InstructorId
            );
            return View(course);
        }

        // POST: /Admin/EditCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course updated successfully!";
                return RedirectToAction("Courses");
            }

            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name",
                model.DeptId
            );
            ViewBag.Instructors = new SelectList(
                await _context.Instructors.ToListAsync(),
                "Id",
                "Name",
                model.InstructorId
            );
            return View(model);
        }

        // GET: /Admin/DeleteCourse
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context
                .Courses.Include(c => c.Department)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Num == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: /Admin/DeleteCourse
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }

            return RedirectToAction("Courses");
        }

        // === DEPARTMENT MANAGEMENT ===

        // GET: /Admin/Departments
        public async Task<IActionResult> Departments()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: /Admin/CreateDepartment
        public IActionResult CreateDepartment()
        {
            return View();
        }

        // POST: /Admin/CreateDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(Department model)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department created successfully!";
                return RedirectToAction("Departments");
            }

            return View(model);
        }

        // GET: /Admin/EditDepartment
        public async Task<IActionResult> EditDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: /Admin/EditDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Department updated successfully!";
                    return RedirectToAction("Departments");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating department: {ex.Message}";
                    return RedirectToAction("EditDepartment", new { id = model.Id });
                }
            }

            return View(model);
        }

        // GET: /Admin/DeleteDepartment
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context
                .Departments.Include(d => d.Students)
                .Include(d => d.Courses)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: /Admin/DeleteDepartment
        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartmentConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }

            return RedirectToAction("Departments");
        }

        // === ADMIN ACCOUNT CREATION ===

        // GET: /Admin/CreateAdmin
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // POST: /Admin/CreateAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true, // Auto-confirm admin emails
                    PhoneNumberConfirmed = true,
                    IsPhoneVerified = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Admin role
                    await _userManager.AddToRoleAsync(user, "Admin");

                    TempData["SuccessMessage"] = $"Admin account '{model.Email}' created successfully!";
                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
