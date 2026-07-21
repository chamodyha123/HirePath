using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HirePathAI.API.DTOs.CompanyOnboarding;
using HirePathAI.API.Services.CompanyOnboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/company-onboarding")]
    public class CompanyOnboardingController : ControllerBase
    {
        private const string PlatformAdminRoles = "Admin,SuperAdmin,PlatformAdmin";
        private readonly ICompanyOnboardingService _service;

        public CompanyOnboardingController(ICompanyOnboardingService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("registrations")]
        public async Task<IActionResult> SubmitRegistration(SubmitCompanyRegistrationDto dto)
        {
            try
            {
                return Ok(await _service.SubmitRegistrationAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = PlatformAdminRoles)]
        [HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrations([FromQuery] string? status = null)
        {
            try
            {
                return Ok(await _service.GetRegistrationRequestsAsync(status));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = PlatformAdminRoles)]
        [HttpPost("registrations/{id:int}/approve")]
        public async Task<IActionResult> Approve(int id, ReviewCompanyRegistrationDto dto)
        {
            try
            {
                return Ok(await _service.ApproveRegistrationAsync(id, CurrentUserId(), dto.Note));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = PlatformAdminRoles)]
        [HttpPost("registrations/{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, ReviewCompanyRegistrationDto dto)
        {
            try
            {
                return Ok(await _service.RejectRegistrationAsync(id, CurrentUserId(), dto.Note));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpPost("members/invite")]
        public async Task<IActionResult> InviteMember(InviteCompanyMemberDto dto)
        {
            try
            {
                return Ok(await _service.InviteMemberAsync(CurrentUserId(), dto));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("invitations/validate")]
        public async Task<IActionResult> ValidateInvitation([FromQuery] string token)
        {
            try
            {
                return Ok(await _service.ValidateInvitationAsync(token));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("invitations/accept")]
        public async Task<IActionResult> AcceptInvitation(AcceptCompanyInvitationDto dto)
        {
            try
            {
                return Ok(await _service.AcceptInvitationAsync(dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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
}
