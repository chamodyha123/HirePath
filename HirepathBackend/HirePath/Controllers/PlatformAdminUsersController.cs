using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/users")]
    [Authorize(Roles = "Admin")]
    public class PlatformAdminUsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public PlatformAdminUsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/platform-admin/users
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

                var isSuspended =
                    user.LockoutEnd.HasValue &&
                    user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    Role = roles.FirstOrDefault() ?? "Unassigned",
                    Roles = roles,
                    Status = isSuspended ? "Suspended" : "Active"
                });
            }

            return Ok(result);
        }

        // GET: api/platform-admin/users/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var isSuspended =
                user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTimeOffset.UtcNow;

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.EmailConfirmed,
                Role = roles.FirstOrDefault() ?? "Unassigned",
                Roles = roles,
                Status = isSuspended ? "Suspended" : "Active"
            });
        }

        // PUT: api/platform-admin/users/1/suspend
        [HttpPut("{id:int}/suspend")]
        public async Task<IActionResult> SuspendUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            if (user.Id == GetCurrentUserId())
            {
                return BadRequest(new
                {
                    message = "You cannot suspend your own administrator account."
                });
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to suspend user.",
                    errors = result.Errors
                        .Select(error => error.Description)
                        .ToArray()
                });
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new
            {
                message = "User suspended successfully.",
                userId = user.Id,
                status = "Suspended"
            });
        }

        // PUT: api/platform-admin/users/1/activate
        [HttpPut("{id:int}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to activate user.",
                    errors = result.Errors
                        .Select(error => error.Description)
                        .ToArray()
                });
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new
            {
                message = "User activated successfully.",
                userId = user.Id,
                status = "Active"
            });
        }

        // PUT: api/platform-admin/users/1/role
        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> UpdateUserRole(
            int id,
            [FromBody] UpdateUserRoleDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new
                {
                    message = "Role is required."
                });
            }

            var requestedRole = request.Role.Trim();

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(existingRole =>
                    existingRole.Name != null &&
                    existingRole.Name.ToLower() ==
                    requestedRole.ToLower());

            if (role?.Name == null)
            {
                return BadRequest(new
                {
                    message = $"Role '{requestedRole}' does not exist."
                });
            }

            var normalizedRoleName = role.Name;

            // Logged-in Admin cannot remove their own Admin role
            if (user.Id == GetCurrentUserId() &&
                !normalizedRoleName.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "You cannot remove your own Admin role."
                });
            }

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            if (currentRoles.Any(currentRole =>
                    currentRole.Equals(
                        normalizedRoleName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return Ok(new
                {
                    message = "User already has the selected role.",
                    userId = user.Id,
                    role = normalizedRoleName,
                    status = "No change"
                });
            }

            if (currentRoles.Any())
            {
                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        currentRoles);

                if (!removeResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to remove current roles.",
                        errors = removeResult.Errors
                            .Select(error => error.Description)
                            .ToArray()
                    });
                }
            }

            var addResult =
                await _userManager.AddToRoleAsync(
                    user,
                    normalizedRoleName);

            if (!addResult.Succeeded)
            {
                // Restore previous roles when assigning the new role fails
                if (currentRoles.Any())
                {
                    await _userManager.AddToRolesAsync(
                        user,
                        currentRoles);
                }

                return BadRequest(new
                {
                    message = "Failed to assign new role.",
                    errors = addResult.Errors
                        .Select(error => error.Description)
                        .ToArray()
                });
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new
            {
                message = "User role updated successfully.",
                userId = user.Id,
                role = normalizedRoleName,
                status = "Role updated"
            });
        }

        // DELETE: api/platform-admin/users/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user =
                await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // Logged-in Admin cannot delete their own account
            if (user.Id == GetCurrentUserId())
            {
                return BadRequest(new
                {
                    message = "You cannot delete your own administrator account."
                });
            }

            var displayName =
                user.FullName ??
                user.UserName ??
                user.Email ??
                $"User {user.Id}";

            // Other Admin accounts can also be deleted
            var result =
                await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to delete user.",
                    errors = result.Errors
                        .Select(error => error.Description)
                        .ToArray()
                });
            }

            return Ok(new
            {
                message = $"{displayName} was deleted successfully.",
                deletedUserId = id,
                status = "Deleted"
            });
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            return int.TryParse(
                claim?.Value,
                out var userId)
                    ? userId
                    : null;
        }
    }

    public class UpdateUserRoleDto
    {
        public string Role { get; set; } =
            string.Empty;
    }
}