using HirePathAI.API.Models.Entities;
using HirePathAI.API.DTOs.PlatformAdmin.Users;
using HirePathAI.API.Data;
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

        // GET: api/platform-admin/users
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? query = null,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null)
        {
            var queryable = _userManager.Users.AsQueryable();

            // 1. Search filter
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                queryable = queryable.Where(u => 
                    u.FullName.ToLower().Contains(query) || 
                    (u.Email ?? "").ToLower().Contains(query) || 
                    (u.UserName ?? "").ToLower().Contains(query));
            }

            // 2. Role filter
            if (!string.IsNullOrEmpty(role) && !string.Equals(role, "All", StringComparison.OrdinalIgnoreCase))
            {
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity != null)
                {
                    var userIdsInRole = await _context.UserRoles
                        .Where(ur => ur.RoleId == roleEntity.Id)
                        .Select(ur => ur.UserId)
                        .ToListAsync();
                    queryable = queryable.Where(u => userIdsInRole.Contains(u.Id));
                }
                else
                {
                    return Ok(new List<object>());
                }
            }

            // 3. Status filter
            if (!string.IsNullOrEmpty(status) && !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(status, "Suspended", StringComparison.OrdinalIgnoreCase))
                {
                    queryable = queryable.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow);
                }
                else if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    queryable = queryable.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.UtcNow);
                }
            }

            var users = await queryable
                .OrderByDescending(user => user.Id)
                .ToListAsync();

            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var isSuspended =
                    user.LockoutEnd.HasValue &&
                    user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                result.Add(new UserResponseDto
                {
                    user.Id,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "Unassigned",
                    Roles = roles,
                    Status = isSuspended ? "Suspended" : "Active",
                    PhoneNumber = user.PhoneNumber
                });
            }

            return Ok(result);
        }
    }
}
