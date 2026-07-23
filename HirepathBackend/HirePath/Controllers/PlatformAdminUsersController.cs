using HirePathAI.API.Models.Entities;
using HirePathAI.API.DTOs.PlatformAdmin.Users;
using HirePathAI.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

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
                var isSuspended = user.LockoutEnd.HasValue &&
                                  user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "Unassigned",
                    Roles = roles,
                    Status = isSuspended ? "Suspended" : "Active",
                    PhoneNumber = user.PhoneNumber
                });
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User not found.");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpper();
            user.NormalizedUserName = dto.Email.ToUpper();
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }

            return Ok(new { message = "User updated successfully." });
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User not found.");

            // Remove existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role
            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }

            return Ok(new { message = "User role updated successfully." });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User not found.");

            var isSuspended = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;
            if (isSuspended)
            {
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }

            return Ok(new
            {
                message = isSuspended ? "User activated successfully." : "User suspended successfully.",
                status = isSuspended ? "Active" : "Suspended"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdValue, out var currentUserId) && currentUserId == id)
            {
                return Conflict(new
                {
                    message = "You cannot delete the account that is currently signed in."
                });
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Several workflow tables intentionally use RESTRICT/NO ACTION to preserve
            // recruitment and administration audit history. Deleting a referenced user
            // directly would cause a SQL foreign-key exception and previously surfaced as
            // an unhelpful HTTP 500. Detect those references first and return a clear 409.
            var blockers = new List<string>();

            var candidateProfileId = await _context.CandidateProfiles
                .Where(profile => profile.UserId == id)
                .Select(profile => (int?)profile.Id)
                .FirstOrDefaultAsync();

            if (candidateProfileId.HasValue)
            {
                var applicationCount = await _context.JobApplications
                    .CountAsync(application => application.CandidateProfileId == candidateProfileId.Value);
                if (applicationCount > 0)
                {
                    blockers.Add($"{applicationCount} job application(s)");
                }
            }

            var scheduledInterviewCount = await _context.Interviews
                .CountAsync(interview => interview.ScheduledByUserId == id);
            if (scheduledInterviewCount > 0)
            {
                blockers.Add($"{scheduledInterviewCount} scheduled interview record(s)");
            }

            var feedbackCount = await _context.InterviewFeedbacks
                .CountAsync(feedback => feedback.SubmittedByUserId == id);
            if (feedbackCount > 0)
            {
                blockers.Add($"{feedbackCount} interview feedback record(s)");
            }

            var evaluationCount = await _context.Evaluations
                .CountAsync(evaluation => evaluation.EvaluatedByUserId == id);
            if (evaluationCount > 0)
            {
                blockers.Add($"{evaluationCount} candidate evaluation record(s)");
            }

            var statusHistoryCount = await _context.ApplicationStatusHistories
                .CountAsync(history => history.ChangedByUserId == id);
            if (statusHistoryCount > 0)
            {
                blockers.Add($"{statusHistoryCount} application status history record(s)");
            }

            var invitationCount = await _context.CompanyInvitations
                .CountAsync(invitation => invitation.InvitedByUserId == id);
            if (invitationCount > 0)
            {
                blockers.Add($"{invitationCount} company invitation record(s)");
            }

            var reviewedRegistrationCount = await _context.CompanyRegistrationRequests
                .CountAsync(request => request.ReviewedByUserId == id);
            if (reviewedRegistrationCount > 0)
            {
                blockers.Add($"{reviewedRegistrationCount} reviewed company registration record(s)");
            }

            if (blockers.Count > 0)
            {
                return Conflict(new
                {
                    message = "This user cannot be permanently deleted because the account is referenced by recruitment or audit records. Suspend the account instead to preserve system history.",
                    dependencies = blockers
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Company membership and candidate profile are configured for cascade
                // deletion. Clearing CompanyId avoids the RESTRICT relationship from User
                // to Company before Identity removes the user row.
                user.CompanyId = null;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new
                    {
                        message = string.Join(" | ", updateResult.Errors.Select(error => error.Description))
                    });
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new
                    {
                        message = string.Join(" | ", result.Errors.Select(error => error.Description))
                    });
                }

                await transaction.CommitAsync();
                return Ok(new { message = "User deleted successfully." });
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return Conflict(new
                {
                    message = "The user is still referenced by related database records and cannot be deleted safely. Suspend the account instead."
                });
            }
        }
    }
}
