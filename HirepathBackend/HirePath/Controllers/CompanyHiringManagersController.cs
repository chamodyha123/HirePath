using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HirePathAI.API.Data;
using HirePathAI.API.DTOs.CompanyOnboarding;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.CompanyOnboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/company/hiring-managers")]
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyHiringManagersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyOnboardingService _onboardingService;

        public CompanyHiringManagersController(
            ApplicationDbContext context,
            ICompanyOnboardingService onboardingService)
        {
            _context = context;
            _onboardingService = onboardingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyId = await GetCompanyIdAsync();
            var managers = await _context.CompanyMembers
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.Role == CompanyMemberRole.HiringManager)
                .OrderBy(x => x.User.FullName)
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.User.FullName,
                    x.User.Email,
                    x.User.UserName,
                    x.IsActive,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(managers);
        }

        [HttpPost]
        public async Task<IActionResult> Invite(InviteHiringManagerDto dto)
        {
            var result = await _onboardingService.InviteMemberAsync(
                CurrentUserId(),
                new InviteCompanyMemberDto
                {
                    Email = dto.Email,
                    FullName = dto.FullName,
                    Role = CompanyMemberRole.HiringManager
                });

            return Ok(result);
        }

        [HttpPut("{membershipId:int}/status")]
        public async Task<IActionResult> UpdateStatus(int membershipId, UpdateMemberStatusDto dto)
        {
            var companyId = await GetCompanyIdAsync();
            var member = await _context.CompanyMembers
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == membershipId &&
                                          x.CompanyId == companyId &&
                                          x.Role == CompanyMemberRole.HiringManager);

            if (member == null)
                return NotFound(new { message = "Hiring Manager not found." });

            member.IsActive = dto.IsActive;
            member.User.IsActive = dto.IsActive;
            member.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = dto.IsActive ? "Hiring Manager activated." : "Hiring Manager deactivated." });
        }

        [HttpDelete("{membershipId:int}")]
        public async Task<IActionResult> Delete(int membershipId)
        {
            var companyId = await GetCompanyIdAsync();
            var member = await _context.CompanyMembers
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == membershipId &&
                                          x.CompanyId == companyId &&
                                          x.Role == CompanyMemberRole.HiringManager);

            if (member == null)
                return NotFound(new { message = "Hiring Manager not found." });

            member.IsActive = false;
            member.User.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hiring Manager removed from the active company team." });
        }

        private async Task<int> GetCompanyIdAsync()
        {
            var userId = CurrentUserId();
            var membership = await _context.CompanyMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId &&
                                          x.Role == CompanyMemberRole.CompanyAdmin &&
                                          x.IsActive);

            if (membership == null)
                throw new UnauthorizedAccessException("Active Company Admin membership not found.");

            return membership.CompanyId;
        }

        private int CurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedAccessException("Invalid user token.");
        }
    }

    public class InviteHiringManagerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateMemberStatusDto
    {
        public bool IsActive { get; set; }
    }
}
