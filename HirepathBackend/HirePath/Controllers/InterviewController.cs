using HirePathAI.API.DTOs.Interview;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewController(IInterviewService service)
        {
            _service = service;
        }

        // SCHEDULE INTERVIEW
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule(ScheduleInterviewDto dto)
        {
            if (!Enum.TryParse<InterviewType>(dto.InterviewType, out var interviewType))
                return BadRequest("Invalid interview type. Use: Online, Physical, or Phone");

            var interview = new Interview
            {
                JobApplicationId = dto.JobApplicationId,
                ScheduledAt = dto.ScheduledAt,
                InterviewType = interviewType,
                MeetingLink = dto.MeetingLink
            };

            var result = await _service.ScheduleAsync(interview);
            return Ok(result);
        }

        // GET BY APPLICATION
        [Authorize]
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetByApplication(int applicationId)
        {
            var result = await _service.GetByApplicationIdAsync(applicationId);
            return Ok(result);
        }

        // GET BY ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Interview not found.");

            return Ok(result);
        }

        // UPDATE INTERVIEW
        [Authorize(Roles = "Recruiter,Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateInterviewDto dto)
        {
            if (!Enum.TryParse<InterviewStatus>(dto.Status, out var status))
                return BadRequest("Invalid status. Use: Scheduled, Completed, Cancelled, Rescheduled, or NoShow");

            var interview = new Interview
            {
                Id = dto.InterviewId,
                ScheduledAt = dto.ScheduledAt,
                MeetingLink = dto.MeetingLink,
                Status = status
            };

            var updated = await _service.UpdateAsync(interview);

            if (!updated)
                return NotFound("Interview not found.");

            return Ok("Interview updated successfully.");
        }

        // EVALUATE INTERVIEW
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("evaluate")]
        public async Task<IActionResult> Evaluate(EvaluateInterviewDto dto)
        {
            var evaluated = await _service.EvaluateAsync(
                dto.InterviewId,
                dto.Score,
                dto.Feedback
            );

            if (!evaluated)
                return NotFound("Interview not found.");

            return Ok("Evaluation submitted successfully.");
        }

        // DELETE INTERVIEW
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Interview not found.");

            return Ok("Interview deleted successfully.");
        }
    }
}