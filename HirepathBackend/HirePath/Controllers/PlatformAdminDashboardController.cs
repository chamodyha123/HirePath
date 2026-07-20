using HirePathAI.API.Services.PlatformAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/dashboard")]
    [Authorize(Roles = "SuperAdmin")]
    public class PlatformAdminDashboardController
        : ControllerBase
    {
        private readonly IPlatformAdminService _service;

        public PlatformAdminDashboardController(
            IPlatformAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult>
            GetDashboard()
        {
            return Ok(
                await _service
                    .GetDashboardAsync());
        }
    }
}