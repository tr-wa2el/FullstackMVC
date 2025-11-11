namespace FullstackMVC.Controllers
{
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using FullstackMVC.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Student Controller - تطبيق لمبادئ SOLID:
    /// - Single Responsibility: مسؤول فقط عن التعامل مع HTTP requests/responses للطلاب
    /// - Open/Closed: مفتوح للتوسع مغلق للتعديل
    /// - Liskov Substitution: يمكن استبدال IStudentService بأي implementation
    /// - Interface Segregation: يعتمد على interfaces محددة
    /// - Dependency Inversion: يعتمد على abstraction (IStudentService) وليس concrete class
    /// </summary>
    [Authorize] // يتطلب تسجيل دخول لكل الـ Actions
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        private readonly IDepartmentService _departmentService;

        // Constructor Injection - تطبيق لمبدأ Dependency Injection
        public StudentController(
            IStudentService studentService,
            IDepartmentService departmentService
        )
        {
            _studentService = studentService;
            _departmentService = departmentService;
        }

        // GET: Student/GetAll
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllWithDepartmentAsync();
            return View(students);
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Add
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يضيفوا students
        public async Task<IActionResult> Add()
        {
            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }

        // POST: Student/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يضيفوا students
        public async Task<IActionResult> Add(Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentService.CreateAsync(student);
                TempData["SuccessMessage"] =
                    $"Student '{student.Name}' has been added successfully!";
                return RedirectToAction(nameof(GetAll));
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", student.DeptId);
            return View(student);
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يعدلوا students
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", student.DeptId);
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يعدلوا students
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.SSN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.UpdateAsync(student);
                    TempData["SuccessMessage"] =
                        $"Student '{student.Name}' has been updated successfully!";
                    return RedirectToAction(nameof(GetAll));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating student: {ex.Message}");
                }
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", student.DeptId);
            return View(student);
        }

        // GET: Student/Delete/5
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يحذفوا students
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")] // Admin أو Instructor يقدروا يحذفوا students
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Student has been deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
            }

            return RedirectToAction(nameof(GetAll));
        }

        // GET: Student/GetAllVM - Using ViewModel
        public async Task<IActionResult> GetAllVM()
        {
            var students = await _studentService.GetAllWithDepartmentAsync();

            var studentsVM = students
                .Select(s => new StudentVM
                {
                    SSN = s.SSN,
                    Name = s.Name,
                    Age = s.Age,
                    Gender = s.Gender,
                    Address = s.Address,
                    Image = s.Image,
                    DepartmentName = s.Department?.Name ?? "N/A",
                    DeptId = s.DeptId,
                })
                .ToList();

            return View(studentsVM);
        }

        // GET: Student/DetailsVM/5 - Using ViewModel
        public async Task<IActionResult> DetailsVM(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            // Create ViewModel
            var studentDetailsVM = new StudentDetailsVM
            {
                SSN = student.SSN,
                Name = student.Name,
                Age = student.Age,
                Gender = student.Gender,
                Address = student.Address,
                Image = student.Image,
                DepartmentName = student.Department?.Name ?? "N/A",
                Message =
                    $"Student Profile: {student.Name} - Department of {student.Department?.Name ?? "Unassigned"}",
                GradesCount = student.Grades?.Count ?? 0,
            };

            // Calculate average grade using service
            studentDetailsVM.AverageGrade = await _studentService.GetStudentAverageGradeAsync(id);

            // Set performance color based on average
            if (studentDetailsVM.AverageGrade.HasValue)
            {
                if (studentDetailsVM.AverageGrade >= 85)
                {
                    studentDetailsVM.PerformanceColor = "green";
                }
                else if (studentDetailsVM.AverageGrade >= 60)
                {
                    studentDetailsVM.PerformanceColor = "orange";
                }
                else
                {
                    studentDetailsVM.PerformanceColor = "red";
                }
            }

            // Map course grades
            if (student.Grades != null && student.Grades.Any())
            {
                studentDetailsVM.CourseGrades = student
                    .Grades.Select(g => new CourseGradeVM
                    {
                        CourseName = g.Course?.Name ?? "N/A",
                        GradeValue = g.GradeValue,
                        GradeStatus =
                            g.GradeValue.HasValue && g.GradeValue.Value >= 60 ? "Pass" : "Fail",
                        GradeColor =
                            g.GradeValue.HasValue && g.GradeValue.Value >= 60 ? "green" : "red",
                    })
                    .ToList();
            }
            else
            {
                studentDetailsVM.PerformanceColor = "gray";
            }

            return View(studentDetailsVM);
        }

        // GET: Student/CoursesVM/5 - View student courses
        public async Task<IActionResult> CoursesVM(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            var courseVM = new StudentCourseVM
            {
                StudentSSN = student.SSN,
                StudentName = student.Name,
                StudentAge = student.Age,
                StudentGender = student.Gender,
                DepartmentName = student.Department?.Name ?? "N/A",
                Message = $"Courses for {student.Name}",
                Courses =
                    student
                        .Grades?.Select(g => new CourseEnrollmentVM
                        {
                            CourseNum = g.Course?.Num ?? 0,
                            CourseName = g.Course?.Name ?? "N/A",
                            CourseTopic = g.Course?.Topic,
                            CourseDescription = g.Course?.Description,
                            MaxDegree = g.Course?.Degree ?? 0,
                            MinDegree = g.Course?.MinDegree ?? 0,
                            StudentGrade = g.GradeValue,
                            GradeStatus =
                                g.GradeValue.HasValue
                                && g.GradeValue.Value >= (g.Course?.MinDegree ?? 50)
                                    ? "Pass"
                                    : "Fail",
                            GradeColor =
                                g.GradeValue.HasValue
                                && g.GradeValue.Value >= (g.Course?.MinDegree ?? 50)
                                    ? "green"
                                    : "red",
                            PercentageScore =
                                g.GradeValue.HasValue && g.Course != null
                                    ? (g.GradeValue.Value / g.Course.Degree) * 100
                                    : null,
                            IsAboveMinimum =
                                g.GradeValue.HasValue
                                && g.GradeValue.Value >= (g.Course?.MinDegree ?? 50),
                        })
                        .ToList() ?? new List<CourseEnrollmentVM>(),
                TotalCoursesEnrolled = student.Grades?.Count ?? 0,
                PassedCoursesCount =
                    student.Grades?.Count(g =>
                        g.GradeValue.HasValue && g.GradeValue.Value >= (g.Course?.MinDegree ?? 50)
                    ) ?? 0,
                FailedCoursesCount =
                    student.Grades?.Count(g =>
                        g.GradeValue.HasValue && g.GradeValue.Value < (g.Course?.MinDegree ?? 50)
                    ) ?? 0,
            };

            // Calculate overall average
            courseVM.OverallAverage = await _studentService.GetStudentAverageGradeAsync(id);

            // Determine performance level
            if (courseVM.OverallAverage.HasValue)
            {
                if (courseVM.OverallAverage >= 85)
                {
                    courseVM.PerformanceLevel = "Excellent";
                    courseVM.PerformanceColor = "green";
                }
                else if (courseVM.OverallAverage >= 70)
                {
                    courseVM.PerformanceLevel = "Good";
                    courseVM.PerformanceColor = "blue";
                }
                else if (courseVM.OverallAverage >= 60)
                {
                    courseVM.PerformanceLevel = "Average";
                    courseVM.PerformanceColor = "orange";
                }
                else
                {
                    courseVM.PerformanceLevel = "Below Average";
                    courseVM.PerformanceColor = "red";
                }
            }

            return View(courseVM);
        }
    }
}
