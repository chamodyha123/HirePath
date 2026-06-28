using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Models.Entities;
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
        public IActionResult Match(Job job, CandidateProfile candidate)
        {
            var result = _aiService.MatchCandidate(job, candidate);

            return Ok(result);
        }

        [HttpPost("rank")]
        public IActionResult Rank(Job job,
            List<CandidateProfile> candidates)
        {
            var result =
                _aiService.RankCandidates(job, candidates);

            return Ok(result);
        }
    }
}