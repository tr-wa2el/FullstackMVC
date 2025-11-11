namespace FullstackMVC.Controllers
{
    using FullstackMVC.Filters;
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize]
    [GlobalExceptionFilter] // Global exception handling
    [ResourceOptimization] // Resource optimization for all actions
    public class InstructorController : Controller
    {
        private readonly IInstructorService _instructorService;

        private readonly IDepartmentService _departmentService;

        // Dependency Injection
        public InstructorController(
            IInstructorService instructorService,
            IDepartmentService departmentService
        )
        {
            _instructorService = instructorService;
            _departmentService = departmentService;
        }

        // GET: Instructor/GetAll
        [CacheResult(durationSeconds: 90)] // Cache for 90 seconds
        [AddCustomHeader("X-Instructor-List", "Accessed")]
        public async Task<IActionResult> GetAll()
        {
            var instructors = await _instructorService.GetAllWithDepartmentAsync();
            return View(instructors);
        }

        // GET: Instructor/Details/5
        [DepartmentLocationAction]
        [AddCustomHeader("X-Instructor-Details", "Viewed")]
        public async Task<IActionResult> Details(int id)
        {
            var instructor = await _instructorService.GetByIdWithDetailsAsync(id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Add
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }

        // POST: Instructor/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Instructor instructor)
        {
            // Check for unique email using service
            if (!await _instructorService.IsEmailUniqueAsync(instructor.Email))
            {
                ModelState.AddModelError("Email", "This email address is already in use.");
            }

            if (ModelState.IsValid)
            {
                await _instructorService.CreateAsync(instructor);
                TempData["SuccessMessage"] =
                    $"Instructor '{instructor.Name}' has been added successfully!";
                return RedirectToAction(nameof(GetAll));
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", instructor.DeptId);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", instructor.DeptId);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Instructor instructor)
        {
            if (id != instructor.Id)
            {
                return NotFound();
            }

            // Check for unique email (excluding current instructor)
            if (!await _instructorService.IsEmailUniqueAsync(instructor.Email, instructor.Id))
            {
                ModelState.AddModelError("Email", "This email address is already in use.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _instructorService.UpdateAsync(instructor);
                    TempData["SuccessMessage"] =
                        $"Instructor '{instructor.Name}' has been updated successfully!";
                    return RedirectToAction(nameof(GetAll));
                }
                catch (Exception ex)
                {
                    if (!await _instructorService.ExistsAsync(instructor.Id))
                    {
                        return NotFound();
                    }
                    ModelState.AddModelError("", $"Error updating instructor: {ex.Message}");
                }
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", instructor.DeptId);
            return View(instructor);
        }

        // GET: Instructor/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var instructor = await _instructorService.GetByIdWithDetailsAsync(id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _instructorService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Instructor has been deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting instructor: {ex.Message}";
            }

            return RedirectToAction(nameof(GetAll));
        }
    }
}
