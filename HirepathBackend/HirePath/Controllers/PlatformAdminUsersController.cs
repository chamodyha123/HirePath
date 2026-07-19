using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/users")]
    [Authorize(Roles = "SuperAdmin")]
    public class PlatformAdminUsersController
        : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(new
            {
                message =
                    "Platform Admin users endpoint."
            });
        }
    }
}