# Filter Usage Guide

This guide demonstrates how to use the various filters in your ASP.NET Core MVC application.

## Available Filters

### 1. Authorization Filter - CheckDepartmentLocationFilter
Checks if department location is "Smart", "Fayoum", or "Cairo" before allowing access.
**Requires** deptId parameter and denies access if:
- deptId is missing
- Department not found
- Location is empty/null
- Location is not in allowed list

### 2. Authorization Filter - AdminUrlAuthorizationFilter (RequireAdminUrl)
Checks if the URL ends with "/admin" before allowing access.
**Denies access if:**
- URL doesn't end with "/admin"
Returns 403 Forbidden with clear message

### 3. Action Filter - ValidateDepartmentLocationFilter
Validates department location when adding or editing departments.
**Only allows locations:** Smart, Fayoum, Cairo

### 4. Action Filter - DepartmentLocationActionFilter
Logs action execution details and validates department location.

### 5. Result Filter - AddCustomHeaderFilter & CacheResultFilter
Adds custom headers to responses and implements caching strategies.

### 6. Resource Filter - ResourceOptimizationFilter & ApiVersionFilter
Optimizes resource usage and validates API versions.

### 7. Exception Filter - GlobalExceptionFilter & ApiExceptionFilter
Handles exceptions gracefully with proper error responses.

## Usage Examples

### Apply Filters to Individual Actions

```csharp
using FullstackMVC.Filters;

public class CourseController : Controller
{
    // Apply single filter
    [CheckDepartmentLocation]
    public IActionResult GetAll()
    {
      // Your code here
    }

    // Apply multiple filters
    [DepartmentLocationAction]
    [AddCustomHeader("X-Course-Key", "CourseDetailsAccessed")]
    [CacheResult(durationSeconds: 120)]
    public IActionResult Details(int id)
    {
        // Your code here
    }

    // Apply resource optimization
    [ResourceOptimization]
    [GlobalExceptionFilter]
    public IActionResult Add(Course course)
 {
        // Your code here
    }
}
```

### Apply Filters to Entire Controller

```csharp
using FullstackMVC.Filters;

[GlobalExceptionFilter]
[DepartmentLocationAction]
public class InstructorController : Controller
{
    // All actions in this controller will have these filters applied
    
    [AddCustomHeader]
    public IActionResult GetAll()
    {
        // Your code here
    }
}
```

### Register Filters Globally in Program.cs

```csharp
builder.Services.AddControllersWithViews(options =>
{
    // Add global filters
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ResourceOptimizationFilter>();
    
    // Or use attributes
options.Filters.Add(typeof(GlobalExceptionFilterAttribute));
});
```

### API Controller with Multiple Filters

```csharp
using FullstackMVC.Filters;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiExceptionFilter]
[ResourceOptimization]
public class CoursesApiController : ControllerBase
{
    [HttpGet]
    [CacheResult(durationSeconds: 300)]
    public IActionResult GetAll()
    {
     // Your code here
    }

    [HttpGet("{id}")]
    [CheckDepartmentLocation]
    public IActionResult Get(int id, [FromQuery] int deptId)
    {
        // Your code here
    }

    [HttpPost]
    [GlobalExceptionFilter]
    public IActionResult Create([FromBody] Course course)
    {
    // Your code here
    }
}
```

## Filter Execution Order

Filters execute in the following order:

1. **Authorization Filters** (CheckDepartmentLocationFilter)
2. **Resource Filters - Executing** (ResourceOptimizationFilter)
3. **Action Filters - Executing** (DepartmentLocationActionFilter)
4. **Action Method Execution**
5. **Action Filters - Executed**
6. **Exception Filters** (if exception occurs)
7. **Result Filters - Executing** (AddCustomHeaderFilter, CacheResultFilter)
8. **Result Execution**
9. **Result Filters - Executed**
10. **Resource Filters - Executed**

## Best Practices

### 1. Use Authorization Filters for Security
```csharp
// Check department location
[CheckDepartmentLocation]
public IActionResult SecureAction(int deptId)
{
    // Only accessible if:
    // 1. deptId parameter is provided
    // 2. Department exists in database
  // 3. Department location is Smart, Fayoum, or Cairo
    // Otherwise returns 403 Forbidden or 404 Not Found
}

// Check URL ends with /admin
[RequireAdminUrl]
public IActionResult AdminPanel()
{
    // Only accessible if URL ends with "/admin"
    // Example: /Department/AdminPanel/admin ✅
    // Example: /Department/AdminPanel ❌ (403 Forbidden)
}
```

### 2. Use Action Filters for Logging
```csharp
[DepartmentLocationAction]
public IActionResult TrackedAction()
{
    // Action execution time will be logged
}
```

### 3. Use Result Filters for Response Modification
```csharp
[AddCustomHeader("X-Custom-Header", "CustomValue")]
[CacheResult(120)] // Cache for 2 minutes
public IActionResult CachedAction()
{
    // Response will have custom headers and caching
}
```

### 4. Use Resource Filters for Early Exit
```csharp
[ResourceOptimization]
public IActionResult OptimizedAction()
{
    // Request will be validated before action execution
}
```

### 5. Use Exception Filters for Error Handling
```csharp
[GlobalExceptionFilter]
public IActionResult RiskyAction()
{
    // Exceptions will be caught and handled gracefully
}
```

## Combining Multiple Filters

```csharp
[CheckDepartmentLocation]          // 1. Check authorization
[ResourceOptimization]  // 2. Optimize resources
[DepartmentLocationAction]         // 3. Log action execution
[AddCustomHeader]   // 4. Add response headers
[CacheResult(300)]       // 5. Cache response
[GlobalExceptionFilter]       // 6. Handle exceptions
public IActionResult ComplexAction(int deptId)
{
    // Your code here with full filter pipeline
}
```

## Testing Filters

### Test Authorization Filter - CheckDepartmentLocation
```csharp
// Pass deptId as query parameter
GET /Course/GetAll?deptId=1

// Or in route
GET /Course/Details/1?deptId=2
```

### Test Authorization Filter - RequireAdminUrl
```csharp
// ✅ This will work
GET /Department/GetAll/admin

// ❌ This will fail (403 Forbidden)
GET /Department/GetAll

// ✅ This will work
GET /Admin/Dashboard/admin

// ❌ This will fail (403 Forbidden)
GET /Admin/Dashboard

// Response on failure:
// Status: 403 Forbidden
// Body: {
//   "message": "Access denied. URL must end with '/admin'...",
//   "currentPath": "/Department/GetAll",
//   "requiredPath": "Must end with: /admin"
// }
```

### Test Response Headers
```csharp
// Check response headers using browser dev tools or Postman
// Look for:
// - X-Action-Result-Key
// - X-Response-Time
// - X-Resource-Processing-Time
// - Cache-Control
// - API-Version
```

## Configuration

### Custom Header Values
```csharp
[AddCustomHeader("X-My-Header", "MyValue")]
public IActionResult MyAction()
{
    // Custom header will be added to response
}
```

### Cache Duration
```csharp
[CacheResult(durationSeconds: 600)] // 10 minutes
public IActionResult LongCachedAction()
{
    // Response cached for 10 minutes
}
```

### API Version
```csharp
[ApiVersion("2.0")]
public class MyApiController : ControllerBase
{
    // Requires API-Version: 2.0 header
}
```

## Common Scenarios

### Scenario 1: Department-Based Access Control
```csharp
[CheckDepartmentLocation]
public IActionResult DepartmentSpecificAction(int deptId)
{
    // Only Smart, Fayoum, and Cairo departments allowed
    // deptId is REQUIRED - will return 403 Forbidden if missing
    // Returns 404 if department not found
}
```

### Scenario 1b: Admin URL Protection
```csharp
[RequireAdminUrl]
public IActionResult AdminDashboard()
{
    // Only accessible via: /Controller/AdminDashboard/admin
    // Any other URL format returns 403 Forbidden
    
    return View();
}

// Or apply to entire controller
[RequireAdminUrl]
public class AdminController : Controller
{
    // All actions require URL ending with /admin
    public IActionResult Index() { } // Must be: /Admin/Index/admin
    public IActionResult Users() { } // Must be: /Admin/Users/admin
}
```

### Scenario 2: Performance Monitoring
```csharp
[ResourceOptimization]
[DepartmentLocationAction]
public IActionResult MonitoredAction()
{
    // Execution time logged
    // Performance metrics in response headers
}
```

### Scenario 3: Cached Public Content
```csharp
[CacheResult(durationSeconds: 3600)] // 1 hour
[AddCustomHeader("X-Content-Type", "Public")]
public IActionResult PublicContent()
{
    // Content cached for 1 hour
}
```

### Scenario 4: Robust API Endpoint
```csharp
[ApiVersion("1.0")]
[ApiExceptionFilter]
[ResourceOptimization]
[CacheResult(300)]
public IActionResult RobustApiEndpoint()
{
    // Version checked
    // Exceptions handled
    // Resources optimized
    // Response cached
}
```

## Debugging Filters

Enable detailed logging in appsettings.json:

```json
{
  "Logging": {
"LogLevel": {
      "Default": "Information",
      "FullstackMVC.Filters": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Check logs for filter execution information:
- Authorization checks
- Action execution times
- Resource processing times
- Exception handling
- Cache operations
