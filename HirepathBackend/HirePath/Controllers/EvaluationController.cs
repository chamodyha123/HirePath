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
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;

        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        // ============ CREATE EVALUATION ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvaluation([FromBody] CreateEvaluationDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var evaluation = await _evaluationService.CreateEvaluationAsync(dto, userId);
                return Ok(new { message = "Evaluation created successfully", evaluation });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET EVALUATION BY ID ============
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvaluation(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _evaluationService.ValidateCompanyAccessAsync(id, userId))
                    return Forbid();

                var evaluation = await _evaluationService.GetEvaluationByIdAsync(id);
                if (evaluation == null)
                    return NotFound(new { error = "Evaluation not found" });

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET EVALUATION BY APPLICATION ============
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetEvaluationByApplication(int applicationId)
        {
            try
            {
                var evaluation = await _evaluationService.GetEvaluationByApplicationAsync(applicationId);
                if (evaluation == null)
                    return NotFound(new { error = "Evaluation not found for this application" });

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET ALL EVALUATIONS BY COMPANY ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpGet("company")]
        public async Task<IActionResult> GetEvaluationsByCompany()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _evaluationService.GetUserCompanyIdAsync(userId);
                if (!user.HasValue)
                    return BadRequest(new { error = "User not associated with a company" });

                var evaluations = await _evaluationService.GetEvaluationsByCompanyAsync(user.Value);
                return Ok(evaluations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ UPDATE EVALUATION ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluation(int id, [FromBody] CreateEvaluationDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _evaluationService.UpdateEvaluationAsync(id, dto, userId);
                if (!result)
                    return NotFound(new { error = "Evaluation not found" });

                return Ok(new { message = "Evaluation updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ FINALIZE EVALUATION ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpPut("{id}/finalize")]
        public async Task<IActionResult> FinalizeEvaluation(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _evaluationService.FinalizeEvaluationAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Evaluation not found" });

                return Ok(new { message = "Evaluation finalized successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ DELETE EVALUATION ============
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _evaluationService.DeleteEvaluationAsync(id, userId);
                if (!result)
                    return NotFound(new { error = "Evaluation not found" });

                return Ok(new { message = "Evaluation deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET EVALUATION SCORES SUMMARY ============
        [Authorize(Roles = "HiringManager,Admin")]
        [HttpGet("summary/{applicationId}")]
        public async Task<IActionResult> GetEvaluationSummary(int applicationId)
        {
            try
            {
                var summary = await _evaluationService.GetEvaluationSummaryAsync(applicationId);
                if (summary == null)
                    return NotFound(new { error = "Evaluation not found for this application" });

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}