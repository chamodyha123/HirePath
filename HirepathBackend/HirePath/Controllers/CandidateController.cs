using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _candidateService.GetAllAsync());
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _candidateService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CandidateProfile profile)
        {
            var result = await _candidateService.CreateAsync(profile);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(CandidateProfile profile)
        {
            var updated = await _candidateService.UpdateAsync(profile);

            if (!updated)
                return NotFound();

            return Ok("Profile Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _candidateService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("Profile Deleted");
        }
    }
}