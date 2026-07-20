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
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewController(IInterviewService service)
        {
            _service = service;
        }

        // SCHEDULE INTERVIEW
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
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
                MeetingLink = dto.MeetingLink,
                Location = dto.Location,
                Panel = dto.Panel,
                Notes = dto.Notes
            };

            var result = await _service.ScheduleAsync(interview, this.GetUserId(), this.IsAdmin());
            return Ok(result);
        }

        // GET BY APPLICATION
        [Authorize]
        [HttpGet("application/{applicationId}")]
        public async Task<IActionResult> GetByApplication(int applicationId)
        {
            var result = await _service.GetByApplicationIdAsync(applicationId, this.GetUserId(), this.IsAdmin());
            return Ok(result);
        }

        // GET BY ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, this.GetUserId(), this.IsAdmin());

            if (result == null)
                return NotFound("Interview not found.");

            return Ok(result);
        }

        // GET ALL INTERVIEWS FOR MY COMPANY
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpGet("company")]
        public async Task<IActionResult> GetByCompany()
        {
            var result = await _service.GetByCompanyAsync(this.GetUserId(), this.IsAdmin());
            return Ok(result);
        }

        // UPDATE INTERVIEW
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateInterviewDto dto)
        {
            InterviewStatus? status = null;
            if (!string.IsNullOrEmpty(dto.Status))
            {
                if (!Enum.TryParse<InterviewStatus>(dto.Status, out var parsedStatus))
                    return BadRequest("Invalid status. Use: Scheduled, Completed, Cancelled, Rescheduled, or NoShow");
                status = parsedStatus;
            }

            try
            {
                var updated = await _service.UpdateAsync(
                    dto.InterviewId,
                    dto.ScheduledAt,
                    dto.MeetingLink,
                    dto.Location,
                    dto.Panel,
                    dto.Notes,
                    status,
                    this.GetUserId(),
                    this.IsAdmin());

                if (!updated)
                    return NotFound("Interview not found.");

                return Ok("Interview updated successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // CANCEL INTERVIEW
        [Authorize(Roles = "Recruiter,Admin,HiringManager")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id, [FromBody] string? notes)
        {
            var cancelled = await _service.CancelAsync(id, notes, this.GetUserId(), this.IsAdmin());

            if (!cancelled)
                return NotFound("Interview not found.");

            return Ok("Interview cancelled successfully.");
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