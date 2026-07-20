using HirePathAI.DTOs;
using HirePathAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HirePathAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly IRecruiterService _service;

        public RecruiterController(IRecruiterService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("companies")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDTO dto)
        {
            var result = await _service.AddCompanyAsync(dto);
            return Ok(result);
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var result = await _service.GetAllCompaniesAsync();
            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin,Recruiter")]
        [HttpPost("departments")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateDTO dto)
        {
            var result = await _service.AddDepartmentAsync(dto);
            return Ok(result);
        }

        [HttpGet("companies/{companyId}/departments")]
        public async Task<IActionResult> GetDepartments(int companyId)
        {
            var result = await _service.GetDepartmentsAsync(companyId);
            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin,Recruiter")]
        [HttpPost("jobs")]
        public async Task<IActionResult> PostJob([FromBody] JobCreateDTO dto)
        {
            var result = await _service.PostJobAsync(dto);
            return Ok(result);
        }

        [HttpGet("jobs/{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var result = await _service.GetJobDetailsAsync(id);
            if (result == null) return NotFound("Job not found.");
            return Ok(result);
        }

        [HttpGet("jobs/search")]
        public async Task<IActionResult> SearchJobs([FromQuery] string? search = null, [FromQuery] string? location = null)
        {
            var result = await _service.SearchJobsAsync(search, location);
            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin,Recruiter")]
        [HttpPut("jobs/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobCreateDTO dto)
        {
            var result = await _service.EditJobAsync(id, dto);
            if (result == null) return NotFound("Job not found to update.");
            return Ok(result);
        }

        [Authorize(Roles = "CompanyAdmin,Recruiter")]
        [HttpDelete("jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var success = await _service.RemoveJobAsync(id);
            if (!success) return NotFound("Job not found.");
            return Ok(new { message = "Job deleted successfully." });
        }

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _service.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}