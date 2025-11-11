namespace FullstackMVC.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Result filter that adds a custom header to the response
    /// </summary>
    public class AddCustomHeaderFilter : IResultFilter
    {
        private readonly ILogger<AddCustomHeaderFilter> _logger;

        private readonly string _headerKey;

        private readonly string _headerValue;

        public AddCustomHeaderFilter(
            ILogger<AddCustomHeaderFilter> logger,
            string headerKey = "X-Action-Result-Key",
            string headerValue = "ResultFilterApplied"
        )
        {
            _logger = logger;
            _headerKey = headerKey;
            _headerValue = headerValue;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Check if response has already started
            if (context.HttpContext.Response.HasStarted)
            {
                _logger.LogWarning(
                    $"Cannot add header '{_headerKey}' because response has already started"
                );
                return;
            }

            try
            {
                // Add custom header before the result is executed
                context.HttpContext.Response.Headers.TryAdd(_headerKey, _headerValue);

                _logger.LogInformation(
                    $"Result filter: Added header '{_headerKey}' = '{_headerValue}'"
                );

                // Add timestamp header
                context.HttpContext.Response.Headers.TryAdd(
                    "X-Response-Time",
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                );

                _logger.LogInformation(
                    $"Executing result for action: {context.ActionDescriptor.DisplayName}"
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Failed to add headers: {ex.Message}");
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation(
                $"Result executed for action: {context.ActionDescriptor.DisplayName}"
            );

            if (context.Canceled)
            {
                _logger.LogWarning($"Result execution was canceled");
            }
        }
    }

    /// <summary>
    /// Result filter attribute that can be applied to controllers or actions
    /// </summary>
    public class AddCustomHeaderAttribute : TypeFilterAttribute
    {
        public AddCustomHeaderAttribute(
            string headerKey = "X-Action-Result-Key",
            string headerValue = "ResultFilterApplied"
        )
            : base(typeof(AddCustomHeaderFilter))
        {
            Arguments = new object[] { headerKey, headerValue };
        }
    }

    /// <summary>
    /// Result filter for response caching
    /// </summary>
    public class CacheResultFilter : IResultFilter
    {
        private readonly int _durationSeconds;

        private readonly ILogger<CacheResultFilter> _logger;

        public CacheResultFilter(ILogger<CacheResultFilter> logger, int durationSeconds = 60)
        {
            _logger = logger;
            _durationSeconds = durationSeconds;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Check if response has already started
            if (context.HttpContext.Response.HasStarted)
            {
                _logger.LogWarning("Cannot add cache headers because response has already started");
                return;
            }

            try
            {
                // Add cache control headers
                context.HttpContext.Response.Headers.TryAdd(
                    "Cache-Control",
                    $"public, max-age={_durationSeconds}"
                );
                context.HttpContext.Response.Headers.TryAdd(
                    "Expires",
                    DateTime.UtcNow.AddSeconds(_durationSeconds).ToString("R")
                );

                _logger.LogInformation($"Cache headers added: max-age={_durationSeconds} seconds");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to add cache headers");
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("Cache result filter completed");
        }
    }

    /// <summary>
    /// Cache result filter attribute
    /// </summary>
    public class CacheResultAttribute : TypeFilterAttribute
    {
        public CacheResultAttribute(int durationSeconds = 60)
            : base(typeof(CacheResultFilter))
        {
            Arguments = new object[] { durationSeconds };
        }
    }
}
