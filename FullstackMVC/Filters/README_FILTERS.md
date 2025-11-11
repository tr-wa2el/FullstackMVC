# ASP.NET Core MVC Filters - Complete Implementation

This document provides a comprehensive overview of all custom filters implemented in the FullstackMVC application.

## Table of Contents
- [Overview](#overview)
- [Filter Types](#filter-types)
- [Implementation Details](#implementation-details)
- [Usage Examples](#usage-examples)
- [Testing Guide](#testing-guide)
- [Best Practices](#best-practices)

## Overview

Filters in ASP.NET Core MVC provide a way to run code before or after specific stages in the request processing pipeline. This project implements all five types of filters with practical use cases.

### Filter Execution Pipeline

```
Request
   ↓
[Authorization Filter] ← Security checks
   ↓
[Resource Filter - Executing] ← Early validation
   ↓
[Action Filter - Executing] ← Pre-action logic
   ↓
[Action Method]
   ↓
[Action Filter - Executed] ← Post-action logic
   ↓
[Exception Filter] ← (if exception)
   ↓
[Result Filter - Executing] ← Pre-response logic
   ↓
[Result Execution]
   ↓
[Result Filter - Executed] ← Post-response logic
   ↓
[Resource Filter - Executed] ← Cleanup
   ↓
Response
```

## Filter Types

### 1. Authorization Filter - `CheckDepartmentLocationFilter`

**Purpose**: Validates that the department location is either "Smart" or "Fayoum" before allowing access.

**File**: `FullstackMVC/Filters/CheckDepartmentLocationFilter.cs`

**When to Use**:
- Securing actions based on department location
- Implementing custom authorization logic
- Early request termination based on business rules

**Usage**:
```csharp
[CheckDepartmentLocation]
public IActionResult SecureAction(int deptId)
{
    // Only accessible if department is in Smart or Fayoum
}
```

**Response**: Returns `403 Forbidden` if location check fails.

### 2. Action Filter - `DepartmentLocationActionFilter`

**Purpose**: Logs action execution details and tracks performance metrics.

**File**: `FullstackMVC/Filters/DepartmentLocationActionFilter.cs`

**Features**:
- Logs action execution start and end
- Measures action execution time
- Validates department location
- Logs warnings for failed location checks

**Usage**:
```csharp
[DepartmentLocationAction]
public IActionResult TrackedAction()
{
  // Action execution time will be logged
}
```

**Logs Output**:
```
[Info] Executing action: CourseController.GetAll
[Info] Department location check passed: Computer Science - smart
[Info] Action executed: CourseController.GetAll - Duration: 245ms
```

### 3. Result Filter - `AddCustomHeaderFilter` & `CacheResultFilter`

**Purpose**: Modifies HTTP response headers and implements caching strategies.

**File**: `FullstackMVC/Filters/ResultFilters.cs`

#### AddCustomHeaderFilter

Adds custom headers to the response for tracking and debugging.

**Usage**:
```csharp
[AddCustomHeader("X-My-Header", "MyValue")]
public IActionResult MyAction()
{
    // Response will include custom headers
}
```

**Response Headers**:
```
X-My-Header: MyValue
X-Response-Time: 2024-01-15 10:30:45
```

#### CacheResultFilter

Implements HTTP caching with Cache-Control headers.

**Usage**:
```csharp
[CacheResult(durationSeconds: 300)] // 5 minutes
public IActionResult CachedAction()
{
    // Response cached for 5 minutes
}
```

**Response Headers**:
```
Cache-Control: public, max-age=300
Expires: Mon, 15 Jan 2024 10:35:45 GMT
```

### 4. Resource Filter - `ResourceOptimizationFilter` & `ApiVersionFilter`

**Purpose**: Early request validation and optimization.

**File**: `FullstackMVC/Filters/ResourceFilters.cs`

#### ResourceOptimizationFilter

Validates request size, handles conditional requests, and tracks resource processing time.

**Features**:
- Request size validation (10MB limit)
- ETag support for conditional requests
- Performance metrics in response headers

**Usage**:
```csharp
[ResourceOptimization]
public IActionResult OptimizedAction()
{
    // Request validated before action execution
}
```

**Response Headers**:
```
X-Resource-Processing-Time: 15ms
```

#### ApiVersionFilter

Validates API version from request headers.

**Usage**:
```csharp
[ApiVersion("1.0")]
public class MyApiController : ControllerBase
{
    // Requires API-Version: 1.0 header
}
```

**Request Header Required**:
```
API-Version: 1.0
```

**Response on Version Mismatch**:
```json
{
  "error": "API version 1.0 required",
  "providedVersion": "2.0",
  "requiredVersion": "1.0"
}
```

### 5. Exception Filter - `GlobalExceptionFilter` & `ApiExceptionFilter`

**Purpose**: Centralized exception handling with proper error responses.

**File**: `FullstackMVC/Filters/ExceptionFilters.cs`

#### GlobalExceptionFilter

Handles exceptions for MVC views with user-friendly error pages.

**Handled Exceptions**:
- `UnauthorizedAccessException` → 403 Access Denied
- `KeyNotFoundException` → 404 Not Found
- `ArgumentException` → 400 Bad Request
- `DBConcurrencyException` → 500 Database Error
- All others → 500 Internal Server Error

**Usage**:
```csharp
[GlobalExceptionFilter]
public IActionResult RiskyAction()
{
    // Exceptions handled gracefully
}
```

#### ApiExceptionFilter

Handles exceptions for API endpoints with JSON responses.

**Usage**:
```csharp
[ApiExceptionFilter]
public IActionResult ApiAction()
{
    // Exceptions returned as JSON
}
```

**Error Response Format**:
```json
{
  "error": "NotFound",
  "message": "The requested resource was not found",
  "timestamp": "2024-01-15T10:30:45Z",
"path": "/api/courses/999",
  "details": {
    "exception": "KeyNotFoundException",
    "stackTrace": "..."
  }
}
```

## Implementation Details

### Dependency Injection

All filters support dependency injection through `TypeFilterAttribute`:

```csharp
public class MyFilterAttribute : TypeFilterAttribute
{
    public MyFilterAttribute() : base(typeof(MyFilter))
    {
    }
}
```

### Filter Registration

#### Option 1: Attribute-Based (Recommended)

```csharp
[GlobalExceptionFilter]
[DepartmentLocationAction]
public class CourseController : Controller
{
    // Filters applied to all actions
}
```

#### Option 2: Global Registration

In `Program.cs`:

```csharp
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ResourceOptimizationFilter>();
});
```

## Usage Examples

### Example 1: Secure Department-Based Access

```csharp
[CheckDepartmentLocation]
public IActionResult DepartmentData(int deptId)
{
var data = _context.Departments
    .Where(d => d.Id == deptId)
        .FirstOrDefault();
 
    return View(data);
}
```

**Test**:
```
GET /Course/DepartmentData?deptId=1
→ 200 OK (if location is Smart or Fayoum)
→ 403 Forbidden (if location is anything else)
```

### Example 2: Performance Monitored Action

```csharp
[ResourceOptimization]
[DepartmentLocationAction]
public IActionResult HeavyOperation()
{
    // Complex operation
    return View();
}
```

**Response Headers**:
```
X-Resource-Processing-Time: 25ms
```

**Logs**:
```
[Info] Resource filter executing for: HeavyOperation
[Info] Executing action: HeavyOperation
[Info] Action executed: HeavyOperation - Duration: 1234ms
[Info] Resource filter executed: HeavyOperation - Total Duration: 1259ms
```

### Example 3: Cached Public API Endpoint

```csharp
[CacheResult(durationSeconds: 600)] // 10 minutes
[AddCustomHeader("X-Content-Type", "Public")]
[ApiExceptionFilter]
public async Task<IActionResult> GetPublicData()
{
 var data = await _context.Courses.ToListAsync();
    return Ok(data);
}
```

**Response Headers**:
```
Cache-Control: public, max-age=600
X-Content-Type: Public
X-Response-Time: 2024-01-15 10:30:45
```

### Example 4: Complete Filter Pipeline

```csharp
[CheckDepartmentLocation]      // 1. Authorization
[ResourceOptimization]          // 2. Resource validation
[DepartmentLocationAction]      // 3. Action tracking
[AddCustomHeader("X-Secure", "Yes")]  // 4. Response headers
[CacheResult(300)]     // 5. Caching
[GlobalExceptionFilter]         // 6. Error handling
public IActionResult SecureOptimizedCachedAction(int deptId)
{
    // Fully protected and optimized action
    var data = GetData(deptId);
    return View(data);
}
```

## Testing Guide

### Test Authorization Filter

```bash
# Test with valid location (Smart)
curl -X GET "https://localhost:5001/api/courses/test-authorization?deptId=1"

# Test with invalid location
curl -X GET "https://localhost:5001/api/courses/test-authorization?deptId=3"
```

### Test Action Filter Logging

Check application logs after calling any action with `[DepartmentLocationAction]`:

```bash
dotnet run
# Call endpoint
# Check console output for logs
```

### Test Result Filter Headers

```bash
curl -I "https://localhost:5001/Course/GetAll"

# Look for headers:
# X-Action-Result-Key: ResultFilterApplied
# X-Response-Time: ...
# Cache-Control: ...
```

### Test Resource Filter

```bash
# Test request size limit
curl -X POST "https://localhost:5001/api/courses" \
  -H "Content-Type: application/json" \
  -d @large_file.json  # > 10MB

# Expected: 400 Bad Request
```

### Test Exception Filter

```bash
# Test exception handling
curl -X GET "https://localhost:5001/api/courses/test-exception"

# Expected JSON response:
# {
#   "error": "InternalServerError",
#   "message": "This is a test exception...",
#   ...
# }
```

### Test API with All Filters

Complete API test using the `CoursesApiController`:

```bash
# Get all courses (cached)
curl -X GET "https://localhost:5001/api/courses"

# Get specific course with dept validation
curl -X GET "https://localhost:5001/api/courses/1?deptId=1"

# Create course (exception handling)
curl -X POST "https://localhost:5001/api/courses" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Course",
    "topic": "Testing",
    "degree": 100,
    "minDegree": 50,
    "deptId": 1
  }'

# Test authorization failure
curl -X GET "https://localhost:5001/api/courses/department/3"
```

## Best Practices

### 1. Filter Ordering

Apply filters in this order for optimal performance:
1. Authorization (reject early)
2. Resource (validate request)
3. Action (track execution)
4. Result (modify response)
5. Exception (catch errors)

### 2. Performance Considerations

- Use caching filters on read-heavy endpoints
- Apply authorization filters at controller level for all actions
- Use resource filters to reject large requests early
- Enable logging selectively (not on every action)

### 3. Error Handling

- Use `GlobalExceptionFilter` for MVC controllers
- Use `ApiExceptionFilter` for API controllers
- Log all exceptions for debugging
- Return user-friendly error messages

### 4. Security

- Always validate department location for sensitive operations
- Use authorization filters for access control
- Implement proper authentication before authorization
- Log security-related events

### 5. Caching Strategy

```csharp
// Read-only, public data: Long cache
[CacheResult(durationSeconds: 3600)] // 1 hour

// Frequently updated data: Short cache
[CacheResult(durationSeconds: 60)] // 1 minute

// User-specific data: No cache or private cache
// Don't use CacheResult or use Response.Cache directives
```

## Debugging

### Enable Debug Logging

In `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "FullstackMVC.Filters": "Debug",
      "FullstackMVC.Controllers": "Debug",
   "Microsoft.AspNetCore": "Warning"
 }
  }
}
```

### Check Filter Execution Order

Add breakpoints in each filter method:
- `OnAuthorization`
- `OnResourceExecuting`/`OnResourceExecuted`
- `OnActionExecuting`/`OnActionExecuted`
- `OnResultExecuting`/`OnResultExecuted`
- `OnException`

### Inspect Response Headers

Use browser DevTools or:

```bash
curl -I "https://localhost:5001/your-endpoint"
```

Look for custom headers added by filters.

## Common Issues and Solutions

### Issue 1: Filter Not Executing

**Problem**: Filter attribute applied but not executing.

**Solution**:
- Ensure filter class implements the correct interface
- Verify `TypeFilterAttribute` is used correctly
- Check dependency injection is working
- Use `[TypeFilter(typeof(YourFilter))]` instead of custom attribute if issues persist

### Issue 2: DbContext Disposed

**Problem**: `ObjectDisposedException` in filter.

**Solution**:
- Don't inject `DbContext` in filter constructor
- Use `TypeFilterAttribute` for dependency injection
- Or create new `DbContext` instance in filter method

### Issue 3: Headers Not Appearing

**Problem**: Custom headers not in response.

**Solution**:
- Apply filter to correct action/controller
- Check filter execution order
- Verify `OnResultExecuting` is being called
- Some middleware might remove headers

### Issue 4: Authorization Always Fails

**Problem**: `CheckDepartmentLocation` always returns 403.

**Solution**:
- Ensure `deptId` is passed correctly (query string, route, or form)
- Check database has departments with "Smart" or "Fayoum" location
- Verify case-insensitive comparison is working
- Check logs for validation details

## Additional Resources

- [ASP.NET Core Filters Documentation](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters)
- [Filter Types and Execution Order](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#filter-types)
- [Custom Filters Tutorial](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#custom-filters)

## Project Files

All filter implementations are located in:
```
FullstackMVC/
├── Filters/
│   ├── CheckDepartmentLocationFilter.cs
│   ├── DepartmentLocationActionFilter.cs
│   ├── ResultFilters.cs
│   ├── ResourceFilters.cs
│   ├── ExceptionFilters.cs
│   ├── FILTER_USAGE_GUIDE.md
│   └── README_FILTERS.md (this file)
├── Controllers/
│   ├── CourseController.cs (with filter examples)
│   ├── InstructorController.cs (with filter examples)
│   └── CoursesApiController.cs (comprehensive API example)
└── Program.cs (global filter registration)
```

## Summary

This implementation provides:
- ✅ **5 types of filters** (Authorization, Action, Result, Resource, Exception)
- ✅ **Dependency injection support** via `TypeFilterAttribute`
- ✅ **Comprehensive logging** for debugging and monitoring
- ✅ **Response caching** for performance optimization
- ✅ **Custom headers** for tracking and debugging
- ✅ **Exception handling** with user-friendly messages
- ✅ **API versioning** support
- ✅ **Performance metrics** in response headers
- ✅ **Department location validation** for business logic
- ✅ **Complete examples** in CoursesApiController

All filters are production-ready and follow ASP.NET Core best practices.
