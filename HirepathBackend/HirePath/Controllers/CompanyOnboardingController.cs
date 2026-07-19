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
        private readonly ICompanyOnboardingService _service;
        public CompanyOnboardingController(ICompanyOnboardingService service) => _service = service;

        [AllowAnonymous, HttpPost("registrations")]
        public async Task<IActionResult> SubmitRegistration(SubmitCompanyRegistrationDto dto) =>
            Ok(await _service.SubmitRegistrationAsync(dto));

        [Authorize(Roles = "Admin"), HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrations([FromQuery] string? status = null) =>
            Ok(await _service.GetRegistrationRequestsAsync(status));

        [Authorize(Roles = "Admin"), HttpPost("registrations/{id:int}/approve")]
        public async Task<IActionResult> Approve(int id, ReviewCompanyRegistrationDto dto) =>
            Ok(await _service.ApproveRegistrationAsync(id, CurrentUserId(), dto.Note));

        [Authorize(Roles = "Admin"), HttpPost("registrations/{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, ReviewCompanyRegistrationDto dto) =>
            Ok(await _service.RejectRegistrationAsync(id, CurrentUserId(), dto.Note));

        [Authorize(Roles = "CompanyAdmin"), HttpPost("members/invite")]
        public async Task<IActionResult> InviteMember(InviteCompanyMemberDto dto) =>
            Ok(await _service.InviteMemberAsync(CurrentUserId(), dto));

        [AllowAnonymous, HttpGet("invitations/validate")]
        public async Task<IActionResult> ValidateInvitation([FromQuery] string token) =>
            Ok(await _service.ValidateInvitationAsync(token));

        [AllowAnonymous, HttpPost("invitations/accept")]
        public async Task<IActionResult> AcceptInvitation(AcceptCompanyInvitationDto dto) =>
            Ok(await _service.AcceptInvitationAsync(dto));

        private int CurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Invalid user token.");
        }
    }
}
