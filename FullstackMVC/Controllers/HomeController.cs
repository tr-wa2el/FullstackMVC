namespace FullstackMVC.Controllers
{
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;
        private readonly ICourseService _courseService;
        private readonly IInstructorService _instructorService;

        public HomeController(
            ILogger<HomeController> logger,
            IStudentService studentService,
            IDepartmentService departmentService,
            ICourseService courseService,
            IInstructorService instructorService)
        {
            _logger = logger;
            _studentService = studentService;
            _departmentService = departmentService;
            _courseService = courseService;
            _instructorService = instructorService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get statistics from database
                var students = await _studentService.GetAllAsync();
                var departments = await _departmentService.GetAllAsync();
                var courses = await _courseService.GetAllAsync();
                var instructors = await _instructorService.GetAllAsync();

                // Set ViewBag data
                ViewBag.StudentCount = students?.Count() ?? 0;
                ViewBag.DepartmentCount = departments?.Count() ?? 0;
                ViewBag.CourseCount = courses?.Count() ?? 0;
                ViewBag.InstructorCount = instructors?.Count() ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading statistics data");

                // Set default values if there's an error
                ViewBag.StudentCount = 8;
                ViewBag.DepartmentCount = 3;
                ViewBag.CourseCount = 5;
                ViewBag.InstructorCount = 4;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
