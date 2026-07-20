using HirePathAI.API.DTOs.Interview;
using HirePathAI.API.Helpers;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class InterviewFeedbackController : ControllerBase
    {
        private readonly IInterviewFeedbackService _service;

        public InterviewFeedbackController(IInterviewFeedbackService service)
        {
            _service = service;
        }

        // SUBMIT INTERVIEW FEEDBACK (Hiring Manager)
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitInterviewFeedbackDto dto)
        {
            if (!Enum.TryParse<RecommendationType>(
                    dto.Recommendation,
                    true,
                    out var recommendation))
            {
                return BadRequest("Invalid recommendation. Use: Hire, Hold, or Reject");
            }

            var feedback = new InterviewFeedback
            {
                InterviewId = dto.InterviewId,
                TechnicalScore = dto.TechnicalScore,
                CommunicationScore = dto.CommunicationScore,
                ProblemSolvingScore = dto.ProblemSolvingScore,
                Comments = dto.Comments,
                Recommendation = recommendation
            };

            try
            {
                var result = await _service.SubmitAsync(
                    feedback,
                    this.GetUserId(),
                    this.IsAdmin());

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // GET FEEDBACK FOR AN APPLICATION
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("{applicationId:int}")]
        public async Task<IActionResult> GetByApplication(int applicationId)
        {
            try
            {
                var result = await _service.GetByApplicationIdAsync(
                    applicationId,
                    this.GetUserId(),
                    this.IsAdmin());

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}