using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HirePathAI.API.DTOs.CompanyOnboarding;
using HirePathAI.API.DTOs.PlatformAdmin.Companies;
using HirePathAI.API.Services.CompanyOnboarding;
using HirePathAI.API.Services.PlatformAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/companies")]
    [Authorize(Roles = "Admin")]
    public class PlatformAdminCompaniesController : ControllerBase
    {
        private readonly IPlatformAdminService _platformAdminService;
        private readonly ICompanyOnboardingService _onboardingService;

        public PlatformAdminCompaniesController(
            IPlatformAdminService platformAdminService,
            ICompanyOnboardingService onboardingService)
        {
            _platformAdminService = platformAdminService;
            _onboardingService = onboardingService;
        }

        // GET: api/platform-admin/companies
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies =
                await _platformAdminService.GetAllCompaniesAsync();

            return Ok(companies);
        }

        // GET: api/platform-admin/companies/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRegistrations()
        {
            try
            {
                var registrations =
                    await _onboardingService
                        .GetRegistrationRequestsAsync("Pending");

                return Ok(registrations);
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
                            "An unexpected error occurred while loading pending registrations."
                    });
            }
        }

        // GET: api/platform-admin/companies/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company =
                await _platformAdminService
                    .GetCompanyByIdAsync(id);

            if (company == null)
            {
                return NotFound(new
                {
                    message = "Company not found."
                });
            }

            return Ok(company);
        }

        // GET: api/platform-admin/companies/registrations
        [HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrationRequests(
            [FromQuery] string? status = null)
        {
            try
            {
                var registrations =
                    await _onboardingService
                        .GetRegistrationRequestsAsync(status);

                return Ok(registrations);
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
                            "An unexpected error occurred while loading registration requests."
                    });
            }
        }

        // PUT: api/platform-admin/companies/registrations/2/approve
        [HttpPut("registrations/{id:int}/approve")]
        public async Task<IActionResult> ApproveRegistration(
            int id,
            [FromBody] ReviewCompanyRegistrationDto request)
        {
            try
            {
                var result =
                    await _onboardingService
                        .ApproveRegistrationAsync(
                            id,
                            CurrentUserId(),
                            request.Note);

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
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while approving the registration."
                    });
            }
        }

        // PUT: api/platform-admin/companies/registrations/3/reject
        [HttpPut("registrations/{id:int}/reject")]
        public async Task<IActionResult> RejectRegistration(
            int id,
            [FromBody] ReviewCompanyRegistrationDto request)
        {
            try
            {
                var result =
                    await _onboardingService
                        .RejectRegistrationAsync(
                            id,
                            CurrentUserId(),
                            request.Note);

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
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "An unexpected error occurred while rejecting the registration."
                    });
            }
        }

        // PUT: api/platform-admin/companies/1/request-information
        [HttpPut("{id:int}/request-information")]
        public async Task<IActionResult> RequestInformation(
            int id,
            [FromBody] RequestInformationDto request)
        {
            try
            {
                var result =
                    await _platformAdminService
                        .RequestInformationAsync(id, request);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Company not found."
                    });
                }

                return Ok(new
                {
                    message =
                        "Additional information requested successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/platform-admin/companies/1/suspend
        [HttpPut("{id:int}/suspend")]
        public async Task<IActionResult> SuspendCompany(int id)
        {
            try
            {
                var result =
                    await _platformAdminService
                        .SuspendCompanyAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Company not found."
                    });
                }

                return Ok(new
                {
                    message = "Company suspended successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/platform-admin/companies/1/activate
        [HttpPut("{id:int}/activate")]
        public async Task<IActionResult> ActivateCompany(int id)
        {
            try
            {
                var result =
                    await _platformAdminService
                        .ActivateCompanyAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Company not found."
                    });
                }

                return Ok(new
                {
                    message = "Company activated successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        private int CurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid administrator token.");
            }

            return userId;
        }
    }
}