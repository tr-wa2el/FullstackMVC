namespace FullstackMVC.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System.Data;

    /// <summary>
    /// Exception filter for handling different types of exceptions
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        private readonly IHostEnvironment _environment;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            IHostEnvironment environment
        )
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(
                context.Exception,
                $"Exception occurred in action: {context.ActionDescriptor.DisplayName}"
            );

            var exceptionType = context.Exception.GetType().Name;
            var message = context.Exception.Message;

            // Handle different exception types
            switch (context.Exception)
            {
                case UnauthorizedAccessException:
                    context.Result = new ViewResult
                    {
                        ViewName = "Error",
                        StatusCode = 403,
                        ViewData = new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary()
                        )
                        {
                            ["Message"] = "You don't have permission to access this resource.",
                            ["Title"] = "Access Denied",
                        },
                    };
                    context.ExceptionHandled = true;
                    break;

                case KeyNotFoundException:
                case InvalidOperationException when message.Contains("not found"):
                    context.Result = new ViewResult
                    {
                        ViewName = "Error",
                        StatusCode = 404,
                        ViewData = new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary()
                        )
                        {
                            ["Message"] = "The requested resource was not found.",
                            ["Title"] = "Not Found",
                        },
                    };
                    context.ExceptionHandled = true;
                    break;

                case ArgumentNullException:
                case ArgumentException:
                    context.Result = new ViewResult
                    {
                        ViewName = "Error",
                        StatusCode = 400,
                        ViewData = new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary()
                        )
                        {
                            ["Message"] = _environment.IsDevelopment()
                                ? message
                                : "Invalid request parameters.",
                            ["Title"] = "Bad Request",
                        },
                    };
                    context.ExceptionHandled = true;
                    break;

                case DBConcurrencyException:
                case InvalidOperationException when message.Contains("database"):
                    context.Result = new ViewResult
                    {
                        ViewName = "Error",
                        StatusCode = 500,
                        ViewData = new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary()
                        )
                        {
                            ["Message"] = "A database error occurred. Please try again later.",
                            ["Title"] = "Database Error",
                        },
                    };
                    context.ExceptionHandled = true;
                    break;

                default:
                    context.Result = new ViewResult
                    {
                        ViewName = "Error",
                        StatusCode = 500,
                        ViewData = new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary()
                        )
                        {
                            ["Message"] = _environment.IsDevelopment()
                                ? $"{exceptionType}: {message}"
                                : "An unexpected error occurred. Please try again later.",
                            ["Title"] = "Error",
                        },
                    };
                    context.ExceptionHandled = true;
                    break;
            }

            // Add exception details to response headers in development
            if (_environment.IsDevelopment())
            {
                context.HttpContext.Response.Headers.TryAdd("X-Exception-Type", exceptionType);
                context.HttpContext.Response.Headers.TryAdd(
                    "X-Exception-Message",
                    message.Replace("\n", " ").Replace("\r", " ")
                );
            }

            _logger.LogInformation(
                $"Exception handled by filter: {exceptionType} - Status Code: {context.HttpContext.Response.StatusCode}"
            );
        }
    }

    /// <summary>
    /// Global exception filter attribute
    /// </summary>
    public class GlobalExceptionFilterAttribute : TypeFilterAttribute
    {
        public GlobalExceptionFilterAttribute()
            : base(typeof(GlobalExceptionFilter))
        {
        }
    }

    /// <summary>
    /// API exception filter for returning JSON responses
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        private readonly IHostEnvironment _environment;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(
                context.Exception,
                $"API Exception occurred: {context.ActionDescriptor.DisplayName}"
            );

            var statusCode = 500;
            var errorType = "InternalServerError";
            var message = "An error occurred while processing your request.";

            switch (context.Exception)
            {
                case UnauthorizedAccessException:
                    statusCode = 403;
                    errorType = "Forbidden";
                    message = "You don't have permission to access this resource.";
                    break;

                case KeyNotFoundException:
                    statusCode = 404;
                    errorType = "NotFound";
                    message = "The requested resource was not found.";
                    break;

                case ArgumentException:
                    statusCode = 400;
                    errorType = "BadRequest";
                    message = context.Exception.Message;
                    break;

                default:
                    if (_environment.IsDevelopment())
                    {
                        message = context.Exception.Message;
                    }
                    break;
            }

            var errorResponse = new
            {
                error = errorType,
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.HttpContext.Request.Path.Value,
                details = _environment.IsDevelopment()
                    ? new
                    {
                        exception = context.Exception.GetType().Name,
                        stackTrace = context.Exception.StackTrace,
                    }
                    : null,
            };

            context.Result = new ObjectResult(errorResponse) { StatusCode = statusCode };

            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// API exception filter attribute
    /// </summary>
    public class ApiExceptionFilterAttribute : TypeFilterAttribute
    {
        public ApiExceptionFilterAttribute()
            : base(typeof(ApiExceptionFilter))
        {
        }
    }
}
