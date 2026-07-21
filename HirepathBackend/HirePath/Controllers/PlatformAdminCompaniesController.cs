using HirePathAI.API.DTOs.PlatformAdmin.Companies;
using HirePathAI.API.Services.PlatformAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers.PlatformAdmin
{
    [ApiController]
    [Route("api/platform-admin/companies")]
    [Authorize(Roles = "SuperAdmin")]
    public class PlatformAdminCompaniesController
        : ControllerBase
    {
        private readonly IPlatformAdminService _service;

        public PlatformAdminCompaniesController(
            IPlatformAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult>
            GetAllCompanies()
        {
            return Ok(
                await _service
                    .GetAllCompaniesAsync());
        }

        [HttpGet("pending")]
        public async Task<IActionResult>
            GetPendingCompanies()
        {
            return Ok(
                await _service
                    .GetPendingCompaniesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>
            GetCompany(int id)
        {
            var company =
                await _service
                    .GetCompanyByIdAsync(id);

            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult>
            ApproveCompany(
                int id,
                ApproveCompanyDto request)
        {
            var result =
                await _service
                    .ApproveCompanyAsync(id, request);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Company approved successfully."
            });
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult>
            RejectCompany(
                int id,
                RejectCompanyDto request)
        {
            var result =
                await _service
                    .RejectCompanyAsync(id, request);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Company rejected successfully."
            });
        }

        [HttpPut("{id}/request-information")]
        public async Task<IActionResult>
            RequestInformation(
                int id,
                RequestInformationDto request)
        {
            var result =
                await _service
                    .RequestInformationAsync(
                        id,
                        request);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Additional information requested."
            });
        }

        [HttpPut("{id}/suspend")]
        public async Task<IActionResult>
            SuspendCompany(int id)
        {
            var result =
                await _service
                    .SuspendCompanyAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Company suspended successfully."
            });
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult>
            ActivateCompany(int id)
        {
            var result =
                await _service
                    .ActivateCompanyAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Company activated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeleteCompany(int id)
        {
            var result =
                await _service
                    .DeleteCompanyAsync(id);

            if (!result)
                return NotFound("Company not found.");

            return Ok(new
            {
                message =
                    "Company deleted successfully."
            });
        }
    }
}