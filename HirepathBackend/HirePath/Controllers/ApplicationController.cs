using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Helpers;
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

        private static readonly HashSet<ApplicationStatus> RecruiterAllowedStatuses = new()
        {
            ApplicationStatus.UnderReview,
            ApplicationStatus.Shortlisted,
            ApplicationStatus.Rejected
        };

        private static readonly HashSet<ApplicationStatus> HiringManagerAllowedStatuses = new()
        {
            ApplicationStatus.Interviewed,
            ApplicationStatus.Offered,
            ApplicationStatus.Hired
        };

        public ApplicationController(IApplicationService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(CreateApplicationDto dto)
        {
            try
            {
                var result = await _service.ApplyAsync(dto.JobId, dto.CoverLetter, dto.ResumeId, this.GetUserId());
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, this.GetUserId(), this.IsAdmin());

            if (result == null)
                return NotFound("Application not found.");

            return Ok(result);
        }

        [Authorize(Roles = "Candidate")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _service.GetMyApplicationsAsync(this.GetUserId());
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("candidate/{candidateProfileId}")]
        public async Task<IActionResult> GetByCandidate(int candidateProfileId)
        {
            try
            {
                var result = await _service.GetByCandidateAsync(candidateProfileId, this.GetUserId(), this.IsAdmin());
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            try
            {
                var result = await _service.GetByJobAsync(jobId, this.GetUserId(), this.IsAdmin());
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("company")]
        public async Task<IActionResult> GetByCompany()
        {
            var result = await _service.GetByCompanyAsync(this.GetUserId(), this.IsAdmin());
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id)
        {
            try
            {
                var history = await _service.GetHistoryAsync(id, this.GetUserId(), this.IsAdmin());
                return Ok(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(UpdateApplicationStatusDto dto)
        {
            var isAdmin = this.IsAdmin();

            if (!isAdmin)
            {
                var isRecruiter = User.IsInRole("Recruiter");
                var isHiringManager = User.IsInRole("HiringManager");

                var allowed =
                    (isRecruiter && RecruiterAllowedStatuses.Contains(dto.Status)) ||
                    (isHiringManager && HiringManagerAllowedStatuses.Contains(dto.Status));

                if (!allowed)
                    return StatusCode(StatusCodes.Status403Forbidden, "Your role is not permitted to set this status.");
            }

            try
            {
                var updated = await _service.UpdateStatusAsync(
                    dto.ApplicationId, dto.Status, dto.Feedback, this.GetUserId(), isAdmin);

                if (!updated)
                    return NotFound("Application not found.");

                return Ok("Application status updated successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut("shortlist")]
        public async Task<IActionResult> Shortlist(WorkflowActionDto dto)
        {
            try
            {
                var updated = await _service.ShortlistAsync(dto.ApplicationId, dto.Notes, this.GetUserId(), this.IsAdmin());
                return updated ? Ok("Candidate shortlisted.") : NotFound("Application not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut("reject")]
        public async Task<IActionResult> Reject(WorkflowActionDto dto)
        {
            try
            {
                var updated = await _service.RejectAsync(dto.ApplicationId, dto.Notes, this.GetUserId(), this.IsAdmin());
                return updated ? Ok("Candidate rejected.") : NotFound("Application not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("interview")]
        public async Task<IActionResult> MarkInterviewCompleted(WorkflowActionDto dto)
        {
            try
            {
                var updated = await _service.MarkInterviewCompletedAsync(dto.ApplicationId, dto.Notes, this.GetUserId(), this.IsAdmin());
                return updated ? Ok("Application marked as interview completed.") : NotFound("Application not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("offer")]
        public async Task<IActionResult> Offer(WorkflowActionDto dto)
        {
            try
            {
                var updated = await _service.SendOfferAsync(dto.ApplicationId, dto.Notes, this.GetUserId(), this.IsAdmin());
                return updated ? Ok("Offer sent.") : NotFound("Application not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("hire")]
        public async Task<IActionResult> Hire(WorkflowActionDto dto)
        {
            try
            {
                var updated = await _service.MarkHiredAsync(dto.ApplicationId, dto.Notes, this.GetUserId(), this.IsAdmin());
                return updated ? Ok("Candidate marked as hired.") : NotFound("Application not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("notes/{id}")]
        public async Task<IActionResult> AddNotes(int id, [FromBody] string notes)
        {
            try
            {
                var updated = await _service.AddRecruiterNotesAsync(id, notes, this.GetUserId(), this.IsAdmin());

                if (!updated)
                    return NotFound("Application not found.");

                return Ok("Notes added successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [Authorize(Roles = "Candidate")]
        [HttpPut("withdraw/{id}")]
        public async Task<IActionResult> Withdraw(int id)
        {
            try
            {
                var withdrawn = await _service.WithdrawAsync(id, this.GetUserId());

                if (!withdrawn)
                    return NotFound("Application not found.");

                return Ok("Application withdrawn successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

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