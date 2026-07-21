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

        public CompanyOnboardingController(
            ICompanyOnboardingService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("registrations")]
        public async Task<IActionResult> SubmitRegistration(
            [FromBody] SubmitCompanyRegistrationDto dto)
        {
            try
            {
                var result =
                    await _service.SubmitRegistrationAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while submitting the company registration."
                    });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrations(
            [FromQuery] string? status = null)
        {
            try
            {
                var result =
                    await _service.GetRegistrationRequestsAsync(status);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while loading company registrations."
                    });
            }
        }

        [Authorize(Roles = "CompanyAdmin")]
        [HttpPost("members/invite")]
        public async Task<IActionResult> InviteMember(
            [FromBody] InviteCompanyMemberDto dto)
        {
            try
            {
                var result =
                    await _service.InviteMemberAsync(
                        CurrentUserId(),
                        dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while inviting the company member."
                    });
            }
        }

        [AllowAnonymous]
        [HttpGet("invitations/validate")]
        public async Task<IActionResult> ValidateInvitation(
            [FromQuery] string token)
        {
            try
            {
                var result =
                    await _service.ValidateInvitationAsync(token);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while validating the invitation."
                    });
            }
        }

        [AllowAnonymous]
        [HttpPost("invitations/accept")]
        public async Task<IActionResult> AcceptInvitation(
            [FromBody] AcceptCompanyInvitationDto dto)
        {
            try
            {
                var result =
                    await _service.AcceptInvitationAsync(dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while accepting the invitation."
                    });
            }
        }

        private int CurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(
                    JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(value, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return id;
        }
    }
}