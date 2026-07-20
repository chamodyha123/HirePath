using HirePathAI.API.DTOs.Evaluation;
using HirePathAI.API.Helpers;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/evaluation")]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _service;

        public EvaluationController(IEvaluationService service)
        {
            _service = service;
        }

        // CREATE OR UPDATE THE EVALUATION FOR AN APPLICATION
        // (Hiring Manager supplies ResumeScore/AIScore, or leaves them null
        // to fall back on the AI match score already stored on the application;
        // InterviewScore is always computed server-side from InterviewFeedback.)
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CreateEvaluationDto dto)
        {
            try
            {
                var evaluation = await _service.CreateOrUpdateAsync(
                    dto.JobApplicationId,
                    dto.ResumeScore,
                    dto.AIScore,
                    this.GetUserId(),
                    this.IsAdmin());

                var response = new EvaluationResponseDto
                {
                    JobApplicationId = evaluation.JobApplicationId,
                    ResumeScore = evaluation.ResumeScore,
                    AIScore = evaluation.AIScore,
                    InterviewScore = evaluation.InterviewScore,
                    OverallScore = evaluation.OverallScore,
                    EvaluatedByUserId = evaluation.EvaluatedByUserId,
                    CreatedAt = evaluation.CreatedAt
                };

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // GET THE EVALUATION FOR AN APPLICATION
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetByApplication(int applicationId)
        {
            var evaluation = await _service.GetByApplicationIdAsync(applicationId, this.GetUserId(), this.IsAdmin());

            if (evaluation == null)
                return NotFound("No evaluation found for this application.");

            var response = new EvaluationResponseDto
            {
                JobApplicationId = evaluation.JobApplicationId,
                ResumeScore = evaluation.ResumeScore,
                AIScore = evaluation.AIScore,
                InterviewScore = evaluation.InterviewScore,
                OverallScore = evaluation.OverallScore,
                EvaluatedByUserId = evaluation.EvaluatedByUserId,
                CreatedAt = evaluation.CreatedAt
            };

            return Ok(response);
        }
    }
}