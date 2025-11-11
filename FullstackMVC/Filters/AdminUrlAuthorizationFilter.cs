namespace FullstackMVC.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Authorization filter that checks if the URL ends with "admin"
    /// Only allows access if URL path ends with "/admin"
    /// </summary>
    public class AdminUrlAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get the request path
            var path = context.HttpContext.Request.Path.Value;

            // Check if path is null or doesn't end with "admin" (case-insensitive)
            if (string.IsNullOrEmpty(path) || !path.EndsWith("/admin", StringComparison.OrdinalIgnoreCase))
            {
                // Deny access with 403 Forbidden
                context.Result = new JsonResult(new
                {
                    message = "Access denied. URL must end with '/admin' to access this resource.",
                    currentPath = path,
                    requiredPath = "Must end with: /admin"
                })
                {
                    StatusCode = 403
                };
            }
        }
    }

    /// <summary>
    /// Attribute to apply AdminUrlAuthorizationFilter to controllers or actions
    /// Usage: [RequireAdminUrl]
    /// </summary>
    public class RequireAdminUrlAttribute : TypeFilterAttribute
    {
        public RequireAdminUrlAttribute()
     : base(typeof(AdminUrlAuthorizationFilter))
        {
        }
    }
}
