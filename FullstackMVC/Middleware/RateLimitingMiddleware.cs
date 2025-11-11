namespace FullstackMVC.Middleware
{
    using System.Collections.Concurrent;

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<RateLimitingMiddleware> _logger;

        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();

        private readonly int _requestLimit;

        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            int requestLimit = 100,
            int timeWindowSeconds = 60
        )
        {
            _next = next;
            _logger = logger;
            _requestLimit = requestLimit;
            _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRequestInfo());

            bool isRateLimited = false;

            lock (clientInfo)
            {
                var now = DateTime.UtcNow;

                // Reset counter if time window has passed
                if (now - clientInfo.WindowStart > _timeWindow)
                {
                    clientInfo.RequestCount = 0;
                    clientInfo.WindowStart = now;
                }

                clientInfo.RequestCount++;

                // Check if limit exceeded
                if (clientInfo.RequestCount > _requestLimit)
                {
                    isRateLimited = true;
                }
            }

            if (isRateLimited)
            {
                _logger.LogWarning($"Rate limit exceeded for client {clientId}.");

                context.Response.StatusCode = 429; // Too Many Requests
                context.Response.ContentType = "text/html";

                var html =
                    @"
<!DOCTYPE html>
<html>
<head>
    <title>Rate Limit Exceeded</title>
    <style>
        body {
   font-family: Arial, sans-serif;
            padding: 20px;
    background-color: #fff3cd;
            color: #856404;
   text-align: center;
     }
        h1 {
 color: #856404;
 }
    </style>
</head>
<body>
    <h1>429 - Too Many Requests</h1>
    <p>You have exceeded the rate limit. Please try again later.</p>
    <p><a href='/'>Return to Home</a></p>
</body>
</html>";

                await context.Response.WriteAsync(html);
                return;
            }

            await _next(context);
        }

        private class ClientRequestInfo
        {
            public int RequestCount { get; set; }

            public DateTime WindowStart { get; set; } = DateTime.UtcNow;
        }
    }
}
