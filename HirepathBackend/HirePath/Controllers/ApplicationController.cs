using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationController(IApplicationService service)
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

        // GET APPLICATION BY ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Application not found.");

            return Ok(result);
        }

        // GET ALL APPLICATIONS BY CANDIDATE
        [Authorize(Roles = "Candidate,Admin")]
        [HttpGet("candidate/{candidateId}")]
        public async Task<IActionResult> GetByCandidate(int candidateId)
        {
            var result = await _service.GetByCandidateAsync(candidateId);
            return Ok(result);
        }

        // GET ALL APPLICATIONS BY JOB
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var result = await _service.GetByJobAsync(jobId);
            return Ok(result);
        }

        // UPDATE APPLICATION STATUS (Hiring Workflow)
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(UpdateApplicationStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(
                dto.ApplicationId,
                dto.Status,
                dto.Feedback
            );

            if (!updated)
                return NotFound("Application not found.");

            return Ok("Application status updated successfully.");
        }

        // ADD RECRUITER NOTES
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("notes/{id}")]
        public async Task<IActionResult> AddNotes(int id, [FromBody] string notes)
        {
            var updated = await _service.AddRecruiterNotesAsync(id, notes);

            if (!updated)
                return NotFound("Application not found.");

            return Ok("Notes added successfully.");
        }

        // WITHDRAW APPLICATION
        [Authorize(Roles = "Candidate")]
        [HttpPut("withdraw/{id}")]
        public async Task<IActionResult> Withdraw(int id)
        {
            var withdrawn = await _service.WithdrawAsync(id);

            if (!withdrawn)
                return NotFound("Application not found.");

            return Ok("Application withdrawn successfully.");
        }

        // DELETE APPLICATION
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Application not found.");

            return Ok("Application deleted successfully.");
        }
    }
}