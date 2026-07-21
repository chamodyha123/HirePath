using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // ← This allows all endpoints without authentication
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aiService, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("parse-resume-text")]
        public async Task<IActionResult> ParseResumeText([FromBody] ResumeParseRequestDto dto)
        {
            try
            {
                var result = await _aiService.ParseResumeAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing resume text");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("parse-resume-file")]
        public async Task<IActionResult> ParseResumeFile([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { error = "No file provided" });

                var result = await _aiService.ParseResumeFileAsync(file);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing resume file");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("extract-skills")]
        public async Task<IActionResult> ExtractSkills([FromBody] string text)
        {
            try
            {
                var result = await _aiService.ExtractSkillsAsync(text);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting skills");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("extract-skills-file")]
        public async Task<IActionResult> ExtractSkillsFromFile([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { error = "No file provided" });

                var skills = await _aiService.ExtractSkillsFromResumeAsync(file);
                return Ok(new { skills });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting skills from file");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("match")]
        public async Task<IActionResult> Match([FromBody] MatchRequestDto dto)
        {
            try
            {
                var result = await _aiService.MatchCandidateAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error matching candidate");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("rank")]
        public async Task<IActionResult> Rank([FromBody] RankRequestDto dto)
        {
            try
            {
                var result = await _aiService.RankCandidatesAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ranking candidates");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromBody] JobRecommendationRequestDto dto)
        {
            try
            {
                var result = await _aiService.GetJobRecommendationsAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendations");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("analytics")]
        public async Task<IActionResult> GetAnalytics([FromBody] RecruitmentAnalyticsRequestDto dto)
        {
            try
            {
                var result = await _aiService.GetRecruitmentAnalyticsAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("report")]
        public async Task<IActionResult> GenerateReport([FromBody] AIReportRequestDto dto)
        {
            try
            {
                var result = await _aiService.GenerateReportAsync(dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
