namespace FullstackMVC.Controllers
{
    using FullstackMVC.Context;
    using FullstackMVC.Filters;
    using FullstackMVC.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// API Controller demonstrating the use of various filters
    /// This controller shows how to apply multiple filters together
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter] // Handle exceptions with JSON responses
    [ResourceOptimization] // Optimize resource usage
    public class CoursesApiController : ControllerBase
    {
        private readonly CompanyContext _context;

        private readonly ILogger<CoursesApiController> _logger;

        public CoursesApiController(CompanyContext context, ILogger<CoursesApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all courses with caching
        /// </summary>
        /// <returns>List of all courses</returns>
        [HttpGet]
        [CacheResult(durationSeconds: 300)] // Cache for 5 minutes
        [AddCustomHeader("X-API-Endpoint", "GetAllCourses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Getting all courses");

            var courses = await _context
                .Courses.Include(c => c.Department)
                .Select(c => new
                {
                    c.Num,
                    c.Name,
                    c.Topic,
                    c.Degree,
                    c.MinDegree,
                    Department = c.Department != null ? c.Department.Name : null,
                    DepartmentLocation = c.Department != null ? c.Department.Location : null,
                })
                .ToListAsync();

            return Ok(
                new
                {
                    success = true,
                    count = courses.Count,
                    data = courses,
                }
            );
        }

        /// <summary>
        /// Get a specific course by ID with department location check
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="deptId">Department ID for validation</param>
        /// <returns>Course details</returns>
        [HttpGet("{id}")]
        [CheckDepartmentLocation] // Requires Smart or Fayoum location
        [DepartmentLocationAction] // Log action details
        [AddCustomHeader("X-API-Endpoint", "GetCourse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id, [FromQuery] int? deptId)
        {
            _logger.LogInformation($"API: Getting course {id}");

            var course = await _context
                .Courses.Include(c => c.Department)
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.Num == id);

            if (course == null)
            {
                return NotFound(
                    new { success = false, message = $"Course with ID {id} not found" }
                );
            }

            var result = new
            {
                course.Num,
                course.Name,
                course.Topic,
                course.Description,
                course.Degree,
                course.MinDegree,
                Department = new
                {
                    course.Department?.Id,
                    course.Department?.Name,
                    course.Department?.Location,
                },
                InstructorCount = course.Instructors?.Count ?? 0,
            };

            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Get courses by department with location filtering
        /// </summary>
        /// <param name="deptId">Department ID</param>
        /// <returns>List of courses in the department</returns>
        [HttpGet("department/{deptId}")]
        [CheckDepartmentLocation]
        [CacheResult(durationSeconds: 180)]
        [AddCustomHeader("X-API-Endpoint", "GetCoursesByDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByDepartment(int deptId)
        {
            var department = await _context.Departments.FindAsync(deptId);

            if (department == null)
            {
                return NotFound(
                    new { success = false, message = $"Department with ID {deptId} not found" }
                );
            }

            var courses = await _context
                .Courses.Where(c => c.DeptId == deptId)
                .Select(c => new
                {
                    c.Num,
                    c.Name,
                    c.Topic,
                    c.Degree,
                    c.MinDegree,
                })
                .ToListAsync();

            return Ok(
                new
                {
                    success = true,
                    department = new
                    {
                        department.Id,
                        department.Name,
                        department.Location,
                    },
                    courseCount = courses.Count,
                    courses,
                }
            );
        }

        /// <summary>
        /// Create a new course (demonstrates exception handling)
        /// </summary>
        /// <param name="course">Course data</param>
        /// <returns>Created course</returns>
        [HttpPost]
        [AddCustomHeader("X-API-Endpoint", "CreateCourse")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v =>
                            v.Errors.Select(e => e.ErrorMessage)
                        ),
                    }
                );
            }

            // This will be caught by ApiExceptionFilter if it fails
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"API: Created course {course.Name}");

            return CreatedAtAction(
                nameof(Get),
                new { id = course.Num, deptId = course.DeptId },
                new
                {
                    success = true,
                    message = "Course created successfully",
                    data = course,
                }
            );
        }

        /// <summary>
        /// Update a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="course">Updated course data</param>
        /// <returns>Updated course</returns>
        [HttpPut("{id}")]
        [DepartmentLocationAction]
        [AddCustomHeader("X-API-Endpoint", "UpdateCourse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Course course)
        {
            if (id != course.Num)
            {
                return BadRequest(new { success = false, message = "ID mismatch" });
            }

            if (!await _context.Courses.AnyAsync(c => c.Num == id))
            {
                return NotFound(
                    new { success = false, message = $"Course with ID {id} not found" }
                );
            }

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"API: Updated course {id}");

            return Ok(
                new
                {
                    success = true,
                    message = "Course updated successfully",
                    data = course,
                }
            );
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [AddCustomHeader("X-API-Endpoint", "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound(
                    new { success = false, message = $"Course with ID {id} not found" }
                );
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"API: Deleted course {id}");

            return Ok(new { success = true, message = "Course deleted successfully" });
        }

        /// <summary>
        /// Test endpoint to demonstrate exception filter
        /// </summary>
        /// <returns>Error response</returns>
        [HttpGet("test-exception")]
        [ApiExceptionFilter]
        public IActionResult TestException()
        {
            // This will be caught by ApiExceptionFilter
            throw new InvalidOperationException(
                "This is a test exception to demonstrate the exception filter"
            );
        }

        /// <summary>
        /// Test endpoint to demonstrate authorization failure
        /// </summary>
        /// <param name="deptId">Department ID</param>
        /// <returns>Success or forbidden</returns>
        [HttpGet("test-authorization")]
        [CheckDepartmentLocation]
        public IActionResult TestAuthorization([FromQuery] int deptId)
        {
            var department = _context.Departments.Find(deptId);

            return Ok(
                new
                {
                    success = true,
                    message = "Authorization passed!",
                    department = new { department?.Name, department?.Location },
                }
            );
        }
    }
}
