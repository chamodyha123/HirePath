using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        // ============ SCHEDULE INTERVIEW ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var interview = await _interviewService.ScheduleInterviewAsync(dto, userId);
                return Ok(new { message = "Interview scheduled successfully", interview });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET INTERVIEW BY ID ============
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInterview(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _interviewService.ValidateCompanyAccessAsync(id, userId))
                    return Forbid();

                var interview = await _interviewService.GetInterviewByIdAsync(id);
                if (interview == null)
                    return NotFound(new { error = "Interview not found" });

                return Ok(interview);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET INTERVIEWS BY APPLICATION ============
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetInterviewsByApplication(int applicationId)
        {
            try
            {
                var interviews = await _interviewService.GetInterviewsByApplicationAsync(applicationId);
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET INTERVIEWS BY COMPANY ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpGet("company")]
        public async Task<IActionResult> GetInterviewsByCompany()
        {
            try
            {
                var userId = GetCurrentUserId();
                // This would need a service to get company ID from user
                // For now, return 501 Not Implemented
                return StatusCode(501, new { error = "Company ID resolution not implemented yet" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ UPDATE INTERVIEW ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateInterview([FromBody] UpdateInterviewDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _interviewService.UpdateInterviewAsync(dto, userId);
                if (!result)
                    return NotFound(new { error = "Interview not found" });

                return Ok(new { message = "Interview updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ CANCEL INTERVIEW ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelInterview(int id, [FromQuery] string? reason)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _interviewService.CancelInterviewAsync(id, reason, userId);
                if (!result)
                    return NotFound(new { error = "Interview not found" });

                return Ok(new { message = "Interview cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ RESCHEDULE INTERVIEW ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpPut("reschedule/{id}")]
        public async Task<IActionResult> RescheduleInterview(int id, [FromBody] DateTime newDateTime)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _interviewService.RescheduleInterviewAsync(id, newDateTime, userId);
                if (!result)
                    return NotFound(new { error = "Interview not found" });

                return Ok(new { message = "Interview rescheduled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ MARK INTERVIEW COMPLETED ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("complete/{id}")]
        public async Task<IActionResult> MarkInterviewCompleted(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _interviewService.MarkInterviewCompletedAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Interview not found" });

                return Ok(new { message = "Interview marked as completed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ MARK INTERVIEW NO-SHOW ============
        [Authorize(Roles = "Recruiter,HiringManager,Admin")]
        [HttpPut("noshow/{id}")]
        public async Task<IActionResult> MarkInterviewNoShow(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _interviewService.MarkInterviewNoShowAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Interview not found" });

                return Ok(new { message = "Interview marked as no-show" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}