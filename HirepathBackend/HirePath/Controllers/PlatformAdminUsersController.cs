using HirePathAI.API.Data;
using HirePathAI.API.DTOs.PlatformAdmin.Users;
using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/users")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PlatformAdminUsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public PlatformAdminUsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));

            _roleManager = roleManager
                ?? throw new ArgumentNullException(nameof(roleManager));

            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/platform-admin/users
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? query = null,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null)
        {
            IQueryable<User> usersQuery = _userManager.Users;

            // Search filter
            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchText = query.Trim();

                usersQuery = usersQuery.Where(user =>
                    EF.Functions.Like(
                        user.FullName ?? string.Empty,
                        $"%{searchText}%") ||
                    EF.Functions.Like(
                        user.Email ?? string.Empty,
                        $"%{searchText}%") ||
                    EF.Functions.Like(
                        user.UserName ?? string.Empty,
                        $"%{searchText}%"));
            }

            // Role filter
            if (!string.IsNullOrWhiteSpace(role) &&
                !string.Equals(
                    role,
                    "All",
                    StringComparison.OrdinalIgnoreCase))
            {
                var roleEntity =
                    await _roleManager.FindByNameAsync(role.Trim());

                if (roleEntity == null)
                {
                    return Ok(new List<UserResponseDto>());
                }

                var userIdsInRole = await _context.UserRoles
                    .Where(userRole =>
                        userRole.RoleId == roleEntity.Id)
                    .Select(userRole =>
                        userRole.UserId)
                    .ToListAsync();

                usersQuery = usersQuery.Where(user =>
                    userIdsInRole.Contains(user.Id));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status) &&
                !string.Equals(
                    status,
                    "All",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(
                    status,
                    "Suspended",
                    StringComparison.OrdinalIgnoreCase))
                {
                    usersQuery = usersQuery.Where(user =>
                        user.LockoutEnd.HasValue &&
                        user.LockoutEnd.Value >
                        DateTimeOffset.UtcNow);
                }
                else if (string.Equals(
                    status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                {
                    usersQuery = usersQuery.Where(user =>
                        !user.LockoutEnd.HasValue ||
                        user.LockoutEnd.Value <=
                        DateTimeOffset.UtcNow);
                }
            }

            var users = await usersQuery
                .OrderByDescending(user => user.Id)
                .ToListAsync();

            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var userRoles =
                    await _userManager.GetRolesAsync(user);

                var isSuspended =
                    user.LockoutEnd.HasValue &&
                    user.LockoutEnd.Value >
                    DateTimeOffset.UtcNow;

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Role = userRoles.FirstOrDefault()
                        ?? "Unassigned",
                    Roles = userRoles.ToList(),
                    Status = isSuspended
                        ? "Suspended"
                        : "Active"
                });
            }

            return Ok(result);
        }
    }
}