// HirePathAI.API/Controllers/PlatformAdmin/PlatformAdminAnalyticsController.cs
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/analytics")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PlatformAdminAnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<PlatformAdminAnalyticsController> _logger;

        public PlatformAdminAnalyticsController(
            IAnalyticsService analyticsService,
            ILogger<PlatformAdminAnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous] // Temporarily allow for testing - REMOVE IN PRODUCTION
        public async Task<IActionResult> GetAnalytics()
        {
            try
            {
                var analytics = await _analyticsService.GetAnalyticsDataAsync();
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching analytics data");
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}