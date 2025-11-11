namespace FullstackMVC.Middleware
{
    using System.Net;

    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env
        )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unhandled exception occurred: {ex.Message}");

                // Only handle the exception if response hasn't started
                if (!context.Response.HasStarted)
                {
                    await HandleExceptionAsync(context, ex);
                }
                else
                {
                    _logger.LogWarning("Cannot handle exception - response has already started");
                    throw; // Re-throw if response has started
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Clear any existing response content if possible
            context.Response.Clear();

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorMessage = _env.IsDevelopment()
                ? $"<h1>Error: {exception.Message}</h1><pre>{exception.StackTrace}</pre>"
                : "<h1>An error occurred while processing your request.</h1><p>Please try again later.</p>";

            var html =
                $@"
                        <!DOCTYPE html>
                        <html>
                            <head>
                                <title>Error</title>
                                <style>
                                    body {{
                                        font-family: Arial, sans-serif;
                                           padding: 20px;
                                      background-color: #f8d7da;
                                      color: #721c24;
                                    }}
                                 h1 {{
                                     color: #721c24;
                                    }}
                              a {{
                                    color: #004085;
                                     text-decoration: none;
                                    }}
                                    a:hover {{
                                         text-decoration: underline;
                                    }}
                                    .error-details {{
      background-color: #fff;
         padding: 15px;
       border-radius: 5px;
           margin-top: 20px;
       border: 1px solid #f5c6cb;
 }}
                                </style>
                            </head>
                        <body>
 <div class='error-details'>
    {errorMessage}
          <p><a href='/'>Return to Home</a></p>
                 </div>
 </body>
     </html>";

            await context.Response.WriteAsync(html);
        }
    }
}
