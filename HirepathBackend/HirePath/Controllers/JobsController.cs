using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _jobService.GetAllAsync());
        }

        // GET: api/Jobs/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveJobs()
        {
            return Ok(await _jobService.GetActiveJobsAsync());
        }

        // GET: api/Jobs/search?keyword=developer
        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            return Ok(await _jobService.SearchJobsAsync(keyword));
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _jobService.GetByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        // POST: api/Jobs
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Job job)
        {
            var result = await _jobService.CreateAsync(job);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/Jobs
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut]
        public async Task<IActionResult> Update(Job job)
        {
            var updated = await _jobService.UpdateAsync(job);

            if (!updated)
                return NotFound();

            return Ok("Job updated successfully.");
        }

        // DELETE: api/Jobs/5
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _jobService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("Job deleted successfully.");
        }
    }
}