using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/users")]
    [Authorize(Roles = "SuperAdmin")]
    public class PlatformAdminUsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public PlatformAdminUsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderByDescending(user => user.Id)
                .ToListAsync();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isSuspended = user.LockoutEnd.HasValue &&
                                  user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "Unassigned",
                    Roles = roles,
                    Status = isSuspended ? "Suspended" : "Active"
                });
            }

            return Ok(result);
        }
    }
}
