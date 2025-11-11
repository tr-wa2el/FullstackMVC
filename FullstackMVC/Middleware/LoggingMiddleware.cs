namespace FullstackMVC.Middleware
{
    using System.Diagnostics;

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;

            _logger.LogInformation(
                $"[REQUEST] {requestMethod} {requestPath} started at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            );

            try
            {
                await _next(context);
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                _logger.LogInformation(
                    $"[RESPONSE] {requestMethod} {requestPath} completed with status {statusCode} in {stopwatch.ElapsedMilliseconds}ms"
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    $"[ERROR] {requestMethod} {requestPath} failed after {stopwatch.ElapsedMilliseconds}ms"
                );
                throw;
            }
        }
    }
}
