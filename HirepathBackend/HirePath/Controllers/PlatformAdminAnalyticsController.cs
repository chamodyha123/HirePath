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

        public PlatformAdminAnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            var analytics = await _analyticsService.GetAnalyticsDataAsync();
            return Ok(analytics);
        }
    }
}
