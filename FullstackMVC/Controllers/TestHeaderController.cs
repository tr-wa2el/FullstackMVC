namespace FullstackMVC.Controllers
{
    using FullstackMVC.Filters;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Test controller to demonstrate custom header functionality
    /// </summary>
    public class TestHeaderController : Controller
    {
        private readonly ILogger<TestHeaderController> _logger;

        public TestHeaderController(ILogger<TestHeaderController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Main index page with all test links
        /// GET: /TestHeader or /TestHeader/Index
        /// </summary>
        public IActionResult Index()
        {
            _logger.LogInformation("TestHeader Index action called");
            return View();
        }

        /// <summary>
        /// Test endpoint with default custom header
        /// GET: /TestHeader/WithDefaultHeader
        /// </summary>
        [AddCustomHeader] // Uses default: X-Action-Result-Key = ResultFilterApplied
        public IActionResult WithDefaultHeader()
        {
            _logger.LogInformation("WithDefaultHeader action called");
            ViewBag.Message = "Check response headers for: X-Action-Result-Key";
            return View("TestHeader");
        }

        /// <summary>
        /// Test endpoint with custom header key and value
        /// GET: /TestHeader/WithCustomHeader
        /// </summary>
        [AddCustomHeader("X-My-Custom-Key", "MyCustomValue")]
        public IActionResult WithCustomHeader()
        {
            _logger.LogInformation("WithCustomHeader action called");
            ViewBag.Message = "Check response headers for: X-My-Custom-Key = MyCustomValue";
            return View("TestHeader");
        }

        /// <summary>
        /// Test endpoint with bonus header
        /// GET: /TestHeader/WithBonusHeader
        /// </summary>
        [AddCustomHeader("X-Bonus-Key", "BonusValue")]
        public IActionResult WithBonusHeader()
        {
            _logger.LogInformation("WithBonusHeader action called");

            // You can also add headers manually in the action
            Response.Headers.TryAdd("X-Manual-Header", "ManualValue");

            ViewBag.Message = "Check response headers for: X-Bonus-Key = BonusValue";
            return View("TestHeader");
        }

        /// <summary>
        /// Test endpoint with multiple headers
        /// GET: /TestHeader/WithMultipleHeaders
        /// </summary>
        [AddCustomHeader("X-Header-1", "Value1")]
        [AddCustomHeader("X-Header-2", "Value2")]
        [AddCustomHeader("X-Header-3", "Value3")]
        public IActionResult WithMultipleHeaders()
        {
            _logger.LogInformation("WithMultipleHeaders action called");
            ViewBag.Message = "Check response headers for multiple X-Header-* entries";
            return View("TestHeader");
        }

        /// <summary>
        /// Test API endpoint that returns JSON with custom header
        /// GET: /TestHeader/ApiWithHeader
        /// </summary>
        [AddCustomHeader("X-API-Key", "ApiResponse")]
        public IActionResult ApiWithHeader()
        {
            _logger.LogInformation("ApiWithHeader action called");

            return Json(
                new
                {
                    success = true,
                    message = "Check response headers for custom headers",
                    timestamp = DateTime.UtcNow,
                    headers = new
                    {
                        customHeader = "X-API-Key: ApiResponse",
                        responseTime = "X-Response-Time: (automatically added)",
                    },
                }
            );
        }

        /// <summary>
        /// Test endpoint without any filter
        /// GET: /TestHeader/WithoutHeader
        /// </summary>
        public IActionResult WithoutHeader()
        {
            _logger.LogInformation("WithoutHeader action called");
            ViewBag.Message = "No custom headers should be added (except standard ones)";
            return View("TestHeader");
        }

        /// <summary>
        /// Test endpoint to view all response headers
        /// GET: /TestHeader/ViewHeaders
        /// </summary>
        [AddCustomHeader("X-Demo-Header", "DemoValue")]
        public IActionResult ViewHeaders()
        {
            _logger.LogInformation("ViewHeaders action called");

            // Add some manual headers for demonstration
            Response.Headers.TryAdd("X-Controller-Name", "TestHeaderController");
            Response.Headers.TryAdd("X-Action-Name", "ViewHeaders");

            return View();
        }

        /// <summary>
        /// Compare filters side by side
        /// GET: /TestHeader/CompareFilters
        /// </summary>
        public IActionResult CompareFilters()
        {
            _logger.LogInformation("CompareFilters action called");
            return View();
        }

        /// <summary>
        /// Test with caching filter
        /// GET: /TestHeader/WithCaching
        /// </summary>
        [CacheResult(durationSeconds: 120)]
        [AddCustomHeader("X-Cached", "Yes")]
        public IActionResult WithCaching()
        {
            _logger.LogInformation("WithCaching action called at {Time}", DateTime.UtcNow);
            ViewBag.Message = "This response is cached for 120 seconds";
            ViewBag.ServerTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            return View("TestHeader");
        }

        /// <summary>
        /// API endpoint to get headers as JSON
        /// GET: /TestHeader/GetHeadersJson
        /// </summary>
        [AddCustomHeader("X-JSON-Response", "true")]
        public IActionResult GetHeadersJson()
        {
            var headers = HttpContext
                .Response.Headers.Where(h => h.Key.StartsWith("X-"))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            return Json(
                new
                {
                    success = true,
                    timestamp = DateTime.UtcNow,
                    customHeaders = headers,
                    message = "Custom headers added by Result Filters",
                }
            );
        }

        /// <summary>
        /// Complete testing guide
        /// GET: /TestHeader/TestingGuide
        /// </summary>
        public IActionResult TestingGuide()
        {
            _logger.LogInformation("TestingGuide action called");
            return View();
        }
    }
}
