using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("parse-resume")]
        public IActionResult ParseResume(ResumeParseRequestDto dto)
        {
            var result = _aiService.ParseResume(dto.ResumeText);
            return Ok(result);
        }

        [HttpPost("match")]
        public IActionResult Match([FromBody] MatchRequestDto dto)
        {
            var result = _aiService.MatchCandidate(dto.Job, dto.Candidate);
            return Ok(result);
        }

        [HttpPost("rank")]
        public IActionResult Rank([FromBody] RankRequestDto dto)
        {
            var result = _aiService.RankCandidates(dto.Job, dto.Candidates);
            return Ok(result);
        }
    }
}