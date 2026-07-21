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
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        // ============ SUBMIT FEEDBACK ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitInterviewFeedbackDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var feedback = await _feedbackService.SubmitFeedbackAsync(dto, userId);
                return Ok(new { message = "Feedback submitted successfully", feedback });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET FEEDBACK BY ID ============
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedback(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _feedbackService.ValidateCompanyAccessAsync(id, userId))
                    return Forbid();

                var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
                if (feedback == null)
                    return NotFound(new { error = "Feedback not found" });

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET FEEDBACK BY APPLICATION ============
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetFeedbackByApplication(int applicationId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbackByApplicationAsync(applicationId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET FEEDBACK BY INTERVIEW ============
        [HttpGet("interview/{interviewId}")]
        public async Task<IActionResult> GetFeedbackByInterview(int interviewId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbackByInterviewAsync(interviewId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET FEEDBACK BY COMPANY ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpGet("company")]
        public async Task<IActionResult> GetFeedbackByCompany()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _feedbackService.GetUserCompanyIdAsync(userId);
                if (!user.HasValue)
                    return BadRequest(new { error = "User not associated with a company" });

                var feedbacks = await _feedbackService.GetFeedbackByCompanyAsync(user.Value);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ UPDATE FEEDBACK ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] SubmitInterviewFeedbackDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _feedbackService.UpdateFeedbackAsync(id, dto, userId);
                if (!result)
                    return NotFound(new { error = "Feedback not found" });

                return Ok(new { message = "Feedback updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ DELETE FEEDBACK ============
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _feedbackService.DeleteFeedbackAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Feedback not found" });

                return Ok(new { message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ SUBMIT FEEDBACK (Mark as submitted) ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("{id}/submit")]
        public async Task<IActionResult> SubmitFeedback(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _feedbackService.SubmitFeedbackAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Feedback not found" });

                return Ok(new { message = "Feedback submitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}