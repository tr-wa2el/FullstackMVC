namespace FullstackMVC.Controllers
{
    using FullstackMVC.Filters;
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using FullstackMVC.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Department Controller - تطبيق لمبادئ SOLID
    /// يعتمد على IDepartmentService بدلاً من CompanyContext مباشرة
    /// </summary>
    [Authorize] // يتطلب تسجيل دخول لكل الـ Actions
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        // Dependency Injection - تطبيق لمبدأ Dependency Inversion
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Department/GetAll
        public async Task<IActionResult> GetAll()
        {
            var depts = await _departmentService.GetAllAsync();
            return View("index", depts);
        }

        // GET: Department/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var dept = await _departmentService.GetByIdWithDetailsAsync(id);

            if (dept == null)
            {
                return NotFound();
            }

            string Msg = $"Hello from Department {dept.Name}";
            ViewBag.Msg = Msg;

            List<string> branches = new List<string> { "ism", "giza", "cairo", "smart" };
            ViewData["brc"] = branches;

            int count = dept.Employees?.Count ?? 0;
            ViewData["count"] = count;
            ViewData["color"] = count > 1 ? "red" : "green";

            return View(dept);
        }

        // GET: Department/DetailsVM/1
        public async Task<IActionResult> detailsVM(int id)
        {
            var dept = await _departmentService.GetByIdWithDetailsAsync(id);

            if (dept == null)
            {
                return NotFound();
            }

            DeptWithEctraInfoVM deptVM = new DeptWithEctraInfoVM
            {
                Name = dept.Name,
                Manager = dept.ManagerName,
                Message = $"Hello from Department {dept.Name}",
                Count = dept.Employees?.Count ?? 0,
                Color = (dept.Employees?.Count ?? 0) > 1 ? "red" : "green",
                Branches = new List<string> { "ism", "giza", "cairo", "smart" },
                empsNames = dept.Employees?.Select(e => e.Name).ToList() ?? new List<string>(),
            };

            return View(deptVM);
        }

        // GET: Department/Add
        [Authorize(Roles = "Admin")] // فقط Admin يقدر يضيف departments
        public IActionResult Add()
        {
            return View();
        }

        // POST: Department/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDepartmentLocation]
        [Authorize(Roles = "Admin")] // فقط Admin يقدر يضيف departments
        public async Task<IActionResult> Add(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.CreateAsync(department);
                    TempData["SuccessMessage"] =
                        $"Department '{department.Name}' has been added successfully!";
                    return RedirectToAction(nameof(GetAll));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error adding department: {ex.Message}");
                }
            }
            return View(department);
        }

        // GET: Department/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDepartmentLocation]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.UpdateAsync(department);
                    TempData["SuccessMessage"] =
                        $"Department '{department.Name}' has been updated successfully!";
                    return RedirectToAction(nameof(GetAll));
                }
                catch (Exception ex)
                {
                    if (!await _departmentService.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    ModelState.AddModelError("", $"Error updating department: {ex.Message}");
                }
            }
            return View(department);
        }

        // GET: Department/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdWithDetailsAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var department = await _departmentService.GetByIdAsync(id);

                if (department == null)
                {
                    return NotFound();
                }

                // Check if department has related records using service
                var canDelete = await _departmentService.CanDeleteDepartmentAsync(id);

                if (!canDelete)
                {
                    TempData["ErrorMessage"] =
                        "Cannot delete this department because it has related records (employees, students, instructors, or courses).";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                await _departmentService.DeleteAsync(id);
                TempData["SuccessMessage"] =
                    $"Department '{department.Name}' has been deleted successfully!";
                return RedirectToAction(nameof(GetAll));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting department: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        public IActionResult test()
        {
            return View();
        }

        //domain/department/testAction/1
        public IActionResult testAction(int id)
        {
            if (id % 2 == 0)
            {
                return Content("Content Result");
            }
            else
            {
                return View("test");
            }
        }

        //Domain/DEpartment/getName
        public ContentResult getName()
        {
            ContentResult res = new ContentResult();
            res.Content = "Hello from MVC Controller";
            return res;
        }

        public JsonResult getJson()
        {
            JsonResult js = new JsonResult(new { name = "Ahmed", id = 10 });
            return js;
        }

        public ViewResult getView()
        {
            ViewResult ve = new ViewResult();
            ve.ViewName = "test";
            return ve;
        }
    }
}
