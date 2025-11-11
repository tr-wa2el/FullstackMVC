namespace FullstackMVC.Filters
{
    using FullstackMVC.Context;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Authorization filter that checks if the department location is Smart, Fayoum, or Cairo
    /// Only allows access if department exists and has an allowed location
    /// </summary>
    public class CheckDepartmentLocationFilter : IAuthorizationFilter
    {
        private readonly CompanyContext _context;

        private readonly string[] _allowedLocations = { "smart", "fayoum" };

        public CheckDepartmentLocationFilter(CompanyContext context)
        {
            _context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Try to get deptId from route data, query string, or form data
            int? deptId = null;

            if (context.RouteData.Values.ContainsKey("deptId"))
            {
                deptId = Convert.ToInt32(context.RouteData.Values["deptId"]);
            }
            else if (context.HttpContext.Request.Query.ContainsKey("deptId"))
            {
                deptId = Convert.ToInt32(context.HttpContext.Request.Query["deptId"]);
            }

            // If no deptId provided, deny access
            if (!deptId.HasValue)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Find the department
            var department = _context.Departments.Find(deptId.Value);

            // If department not found, deny access
            if (department == null)
            {
                context.Result = new NotFoundObjectResult(new { message = "Department not found" });
                return;
            }

            // Check if location is null or empty
            if (string.IsNullOrWhiteSpace(department.Location))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if location is in allowed list
            var location = department.Location.ToLower().Trim();
            if (!_allowedLocations.Contains(location))
            {
                context.Result = new JsonResult(
                    new
                    {
                        message = $"Access denied. Department location '{department.Location}' is not allowed. Allowed locations: {string.Join(", ", _allowedLocations)}",
                    }
                )
                {
                    StatusCode = 403,
                };
                return;
            }
        }
    }

    /// <summary>
    /// Authorization filter attribute that can be applied to controllers or actions
    /// </summary>
    public class CheckDepartmentLocationAttribute : TypeFilterAttribute
    {
        public CheckDepartmentLocationAttribute()
            : base(typeof(CheckDepartmentLocationFilter))
        {
        }
    }
}
