namespace FullstackMVC.Filters
{
    using FullstackMVC.Context;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Diagnostics;

    /// <summary>
    /// Action filter that checks department location and logs action execution time
    /// </summary>
    public class DepartmentLocationActionFilter : IActionFilter
    {
        private readonly CompanyContext _context;

        private readonly ILogger<DepartmentLocationActionFilter> _logger;

        private Stopwatch? _stopwatch;

        public DepartmentLocationActionFilter(
            CompanyContext context,
            ILogger<DepartmentLocationActionFilter> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();

            // Try to get deptId from action parameters
            if (context.ActionArguments.ContainsKey("deptId"))
            {
                var deptId = context.ActionArguments["deptId"] as int?;

                if (deptId.HasValue)
                {
                    var department = _context.Departments.Find(deptId.Value);

                    if (department != null)
                    {
                        var location = department.Location?.ToLower();

                        if (location == "smart" || location == "fayoum")
                        {
                            _logger.LogInformation(
                                $"Department location check passed: {department.Name} - {location}"
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Department location check failed: {department.Name} - {location}"
                            );
                        }
                    }
                }
            }

            _logger.LogInformation($"Executing action: {context.ActionDescriptor.DisplayName}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();

            _logger.LogInformation(
                $"Action executed: {context.ActionDescriptor.DisplayName} - Duration: {_stopwatch?.ElapsedMilliseconds}ms"
            );

            if (context.Exception != null)
            {
                _logger.LogError(
                    context.Exception,
                    $"Action failed: {context.ActionDescriptor.DisplayName}"
                );
            }
        }
    }

    /// <summary>
    /// Action filter attribute that can be applied to controllers or actions
    /// </summary>
    public class DepartmentLocationActionAttribute : TypeFilterAttribute
    {
        public DepartmentLocationActionAttribute()
            : base(typeof(DepartmentLocationActionFilter))
        {
        }
    }
}
