namespace FullstackMVC.Filters
{
    using FullstackMVC.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Action filter that validates department location when adding or editing departments
    /// Only allows locations: Smart, Fayoum, Cairo
    /// </summary>
    public class ValidateDepartmentLocationFilter : IActionFilter
    {
        private readonly string[] _allowedLocations = { "smart", "fayoum", "cairo" };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the action has a Department parameter
            if (
                context.ActionArguments.TryGetValue("department", out var deptObj)
                && deptObj is Department department
            )
            {
                // Check if location is null or empty
                if (string.IsNullOrWhiteSpace(department.Location))
                {
                    context.ModelState.AddModelError("Location", "Department location is required");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                    return;
                }

                // Check if location is in allowed list
                var location = department.Location.ToLower().Trim();
                if (!_allowedLocations.Contains(location))
                {
                    context.ModelState.AddModelError(
                        "Location",
                        $"Department location '{department.Location}' is not allowed. Allowed locations: {string.Join(", ", _allowedLocations)}"
                    );
                    context.Result = new BadRequestObjectResult(context.ModelState);
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    /// <summary>
    /// Attribute to apply ValidateDepartmentLocationFilter to controllers or actions
    /// </summary>
    public class ValidateDepartmentLocationAttribute : TypeFilterAttribute
    {
        public ValidateDepartmentLocationAttribute()
            : base(typeof(ValidateDepartmentLocationFilter))
        {
        }
    }
}
