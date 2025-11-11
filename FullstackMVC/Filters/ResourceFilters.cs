namespace FullstackMVC.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Diagnostics;

    /// <summary>
    /// Resource filter for request optimization and early response
    /// </summary>
    public class ResourceOptimizationFilter : IResourceFilter
    {
        private readonly ILogger<ResourceOptimizationFilter> _logger;

        private Stopwatch? _stopwatch;

        public ResourceOptimizationFilter(ILogger<ResourceOptimizationFilter> logger)
        {
            _logger = logger;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                $"Resource filter executing for: {context.ActionDescriptor.DisplayName}"
            );

            // Example: Check if result is already cached (simple demonstration)
            var cacheKey = context.HttpContext.Request.Path.ToString();

            // Check for conditional requests (ETag)
            if (context.HttpContext.Request.Headers.ContainsKey("If-None-Match"))
            {
                var etag = context.HttpContext.Request.Headers["If-None-Match"].ToString();
                _logger.LogInformation($"Conditional request detected with ETag: {etag}");

                // If content hasn't changed, return 304 Not Modified
                // This is a simplified example - in real scenarios, you'd check actual content
                // context.Result = new StatusCodeResult(304);
                // return;
            }

            // Check request size limits
            var contentLength = context.HttpContext.Request.ContentLength;
            if (contentLength.HasValue && contentLength.Value > 10 * 1024 * 1024) // 10MB limit
            {
                _logger.LogWarning($"Request too large: {contentLength.Value} bytes");
                context.Result = new BadRequestObjectResult(
                    new { error = "Request payload too large" }
                );
                return;
            }

            _logger.LogInformation("Resource filter validation passed");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _stopwatch?.Stop();

            _logger.LogInformation(
                $"Resource filter executed: {context.ActionDescriptor.DisplayName} - Total Duration: {_stopwatch?.ElapsedMilliseconds}ms"
            );

            if (context.Exception != null)
            {
                _logger.LogError(
                    context.Exception,
                    $"Resource filter caught exception for: {context.ActionDescriptor.DisplayName}"
                );
            }

            // Add performance metric to response headers (only if response hasn't started)
            if (_stopwatch != null && !context.HttpContext.Response.HasStarted)
            {
                try
                {
                    context.HttpContext.Response.Headers.TryAdd(
                        "X-Resource-Processing-Time",
                        $"{_stopwatch.ElapsedMilliseconds}ms"
                    );
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to add performance header");
                }
            }
        }
    }

    /// <summary>
    /// Resource filter attribute that can be applied to controllers or actions
    /// </summary>
    public class ResourceOptimizationAttribute : TypeFilterAttribute
    {
        public ResourceOptimizationAttribute()
            : base(typeof(ResourceOptimizationFilter))
        {
        }
    }

    /// <summary>
    /// Resource filter for API versioning check
    /// </summary>
    public class ApiVersionFilter : IResourceFilter
    {
        private readonly ILogger<ApiVersionFilter> _logger;

        private readonly string _requiredVersion;

        public ApiVersionFilter(ILogger<ApiVersionFilter> logger, string requiredVersion = "1.0")
        {
            _logger = logger;
            _requiredVersion = requiredVersion;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // Check API version header
            if (context.HttpContext.Request.Headers.ContainsKey("API-Version"))
            {
                var version = context.HttpContext.Request.Headers["API-Version"].ToString();

                if (version != _requiredVersion)
                {
                    _logger.LogWarning(
                        $"Invalid API version: {version}, required: {_requiredVersion}"
                    );
                    context.Result = new BadRequestObjectResult(
                        new
                        {
                            error = $"API version {_requiredVersion} required",
                            providedVersion = version,
                            requiredVersion = _requiredVersion,
                        }
                    );
                    return;
                }

                _logger.LogInformation($"API version validated: {version}");
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Add version to response (only if response hasn't started)
            if (!context.HttpContext.Response.HasStarted)
            {
                try
                {
                    context.HttpContext.Response.Headers.TryAdd("API-Version", _requiredVersion);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Failed to add API version header");
                }
            }
        }
    }

    /// <summary>
    /// API version filter attribute
    /// </summary>
    public class ApiVersionAttribute : TypeFilterAttribute
    {
        public ApiVersionAttribute(string requiredVersion = "1.0")
            : base(typeof(ApiVersionFilter))
        {
            Arguments = new object[] { requiredVersion };
        }
    }
}
