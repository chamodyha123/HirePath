using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public JobApplicationsController(IJobApplicationService service)
        {
            _service = service;
        }

        // APPLY FOR JOB
        [Authorize(Roles = "Candidate")]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(CreateApplicationDto dto)
        {
            var application = new JobApplication
            {
                JobId = dto.JobId,
                CandidateProfileId = dto.CandidateProfileId,
                CoverLetter = dto.CoverLetter
            };

            var result = await _service.ApplyAsync(application);
            return Ok(result);
        }

        // GET BY CANDIDATE
        [HttpGet("candidate/{id}")]
        public async Task<IActionResult> GetByCandidate(int id)
        {
            var result = await _service.GetByCandidateAsync(id);
            return Ok(result);
        }

        // GET BY JOB
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var result = await _service.GetByJobAsync(jobId);
            return Ok(result);
        }

        // UPDATE STATUS
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(UpdateApplicationStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(
                dto.ApplicationId,
                dto.Status,
                dto.Feedback
            );

            if (!updated)
                return NotFound();

            return Ok("Status updated successfully.");
        }

        // DELETE APPLICATION
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("Deleted successfully.");
        }
    }
}