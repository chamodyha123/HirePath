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
        private readonly ILogger<JobsController> _logger;

        public JobsController(
            IJobService jobService,
            ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var jobs = await _jobService.GetAllAsync();
                return Ok(jobs.Select(ToJobResponse));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to load jobs.");
                return Problem(
                    title: "Unable to load jobs",
                    detail: "An error occurred while reading jobs from the database.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Jobs/active
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveJobs()
        {
            try
            {
                var jobs = await _jobService.GetActiveJobsAsync();
                return Ok(jobs.Select(ToJobResponse));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to load active jobs.");
                return Problem(
                    title: "Unable to load active jobs",
                    detail: "An error occurred while reading active jobs from the database. Check the API console for the detailed database error.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Jobs/search?keyword=developer
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string? keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    var activeJobs = await _jobService.GetActiveJobsAsync();
                    return Ok(activeJobs.Select(ToJobResponse));
                }

                var jobs = await _jobService.SearchJobsAsync(keyword.Trim());
                return Ok(jobs.Select(ToJobResponse));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to search jobs using keyword {Keyword}.", keyword);
                return Problem(
                    title: "Unable to search jobs",
                    detail: "An error occurred while searching jobs.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Jobs/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _jobService.GetByIdAsync(id);

            if (job == null)
            {
                return NotFound(new { message = "Job not found." });
            }

            return Ok(ToJobResponse(job));
        }

        // POST: api/Jobs
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Job job)
        {
            var result = await _jobService.CreateAsync(job);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                ToJobResponse(result));
        }

        // PUT: api/Jobs
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Job job)
        {
            var updated = await _jobService.UpdateAsync(job);

            if (!updated)
            {
                return NotFound(new { message = "Job not found." });
            }

            return Ok(new { message = "Job updated successfully." });
        }

        // DELETE: api/Jobs/5
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _jobService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Job not found." });
            }

            return Ok(new { message = "Job deleted successfully." });
        }

        private static object ToJobResponse(Job job)
        {
            return new
            {
                job.Id,
                job.Title,
                job.Description,
                employmentType = job.EmploymentType.ToString(),
                workMode = job.WorkMode.ToString(),
                job.Location,
                experienceLevel = job.ExperienceLevel.ToString(),
                job.SalaryMin,
                job.SalaryMax,
                job.ApplicationDeadline,
                job.IsActive,
                job.CompanyId,
                companyName = job.Company?.Name,
                company = job.Company == null
                    ? null
                    : new
                    {
                        job.Company.Id,
                        job.Company.Name,
                        job.Company.LogoUrl,
                        job.Company.Location
                    },
                job.DepartmentId,
                departmentName = job.Department?.Name,
                job.CreatedAt,
                job.UpdatedAt
            };
        }
    }
}
