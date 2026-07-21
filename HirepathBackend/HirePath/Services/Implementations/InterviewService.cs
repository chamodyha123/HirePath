using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationService _applicationService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InterviewService(
            IInterviewRepository interviewRepository,
            IApplicationRepository applicationRepository,
            IApplicationService applicationService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _interviewRepository = interviewRepository;
            _applicationRepository = applicationRepository;
            _applicationService = applicationService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        public async Task<Interview> ScheduleInterviewAsync(ScheduleInterviewDto dto, int userId)
        {
            // Verify application exists
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application == null)
                throw new ArgumentException("Application not found");

            // Verify user has access to this application's company
            var companyId = await _applicationRepository.GetCompanyIdByApplicationIdAsync(dto.ApplicationId);
            if (!companyId.HasValue)
                throw new ArgumentException("Company not found");

            var userCompanyId = await _userRepository.GetUserCompanyIdAsync(userId);
            if (userCompanyId != companyId)
                throw new UnauthorizedAccessException("You don't have access to this application");

            // Check if interview already scheduled for this application
            var existingInterviews = await _interviewRepository.GetInterviewsByApplication(dto.ApplicationId);
            if (existingInterviews.Any(i => i.Status == InterviewStatus.Scheduled))
                throw new InvalidOperationException("An interview is already scheduled for this application");

            var interview = new Interview
            {
                JobApplicationId = dto.ApplicationId,
                ScheduledAt = dto.ScheduledAt,
                InterviewType = dto.InterviewType,
                MeetingLink = dto.MeetingLink,
                Status = InterviewStatus.Scheduled,
                Notes = dto.Notes,
                Location = dto.Location,
                PanelMembers = dto.PanelMembers,
                CreatedBy = userId
            };

            await _interviewRepository.AddAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            // Update application status to InterviewScheduled
            await _applicationService.UpdateStatusAsync(
                dto.ApplicationId,
                ApplicationStatus.InterviewScheduled,
                $"Interview scheduled for {dto.ScheduledAt:yyyy-MM-dd HH:mm}",
                userId
            );

            return interview;
        }

        public async Task<Interview?> GetInterviewByIdAsync(int id)
        {
            return await _interviewRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Interview>> GetInterviewsByApplicationAsync(int applicationId)
        {
            return await _interviewRepository.GetInterviewsByApplication(applicationId);
        }

        public async Task<IEnumerable<Interview>> GetInterviewsByCompanyAsync(int companyId)
        {
            return await _interviewRepository.GetInterviewsByCompany(companyId);
        }

        public async Task<bool> UpdateInterviewAsync(UpdateInterviewDto dto, int userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(dto.InterviewId);
            if (interview == null)
                return false;

            // Verify company access
            if (!await ValidateCompanyAccessAsync(dto.InterviewId, userId))
                throw new UnauthorizedAccessException("You don't have access to this interview");

            // Can only update scheduled interviews
            if (interview.Status != InterviewStatus.Scheduled && interview.Status != InterviewStatus.Rescheduled)
                throw new InvalidOperationException($"Cannot update interview with status {interview.Status}");

            interview.ScheduledAt = dto.ScheduledAt ?? interview.ScheduledAt;
            interview.InterviewType = dto.InterviewType ?? interview.InterviewType;
            interview.MeetingLink = dto.MeetingLink ?? interview.MeetingLink;
            interview.Location = dto.Location ?? interview.Location;
            interview.PanelMembers = dto.PanelMembers ?? interview.PanelMembers;
            interview.Notes = dto.Notes ?? interview.Notes;
            interview.UpdatedAt = DateTime.UtcNow;
            interview.UpdatedBy = userId;

            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelInterviewAsync(int interviewId, string? reason, int userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                return false;

            // Verify company access
            if (!await ValidateCompanyAccessAsync(interviewId, userId))
                throw new UnauthorizedAccessException("You don't have access to this interview");

            // Can only cancel scheduled interviews
            if (interview.Status != InterviewStatus.Scheduled && interview.Status != InterviewStatus.Rescheduled)
                throw new InvalidOperationException($"Cannot cancel interview with status {interview.Status}");

            interview.Status = InterviewStatus.Cancelled;
            interview.Notes = $"{interview.Notes}\nCancelled: {reason ?? "No reason provided"}";
            interview.UpdatedAt = DateTime.UtcNow;
            interview.UpdatedBy = userId;

            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            // Update application status back to Shortlisted
            await _applicationService.UpdateStatusAsync(
                interview.JobApplicationId,
                ApplicationStatus.Shortlisted,
                $"Interview cancelled: {reason}",
                userId
            );

            return true;
        }

        public async Task<bool> RescheduleInterviewAsync(int interviewId, DateTime newDateTime, int userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                return false;

            // Verify company access
            if (!await ValidateCompanyAccessAsync(interviewId, userId))
                throw new UnauthorizedAccessException("You don't have access to this interview");

            // Can only reschedule scheduled interviews
            if (interview.Status != InterviewStatus.Scheduled && interview.Status != InterviewStatus.Rescheduled)
                throw new InvalidOperationException($"Cannot reschedule interview with status {interview.Status}");

            var oldDateTime = interview.ScheduledAt;
            interview.ScheduledAt = newDateTime;
            interview.Status = InterviewStatus.Rescheduled;
            interview.Notes = $"{interview.Notes}\nRescheduled from {oldDateTime:yyyy-MM-dd HH:mm} to {newDateTime:yyyy-MM-dd HH:mm}";
            interview.UpdatedAt = DateTime.UtcNow;
            interview.UpdatedBy = userId;

            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkInterviewCompletedAsync(int interviewId, int userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                return false;

            if (interview.Status != InterviewStatus.Scheduled && interview.Status != InterviewStatus.Rescheduled)
                throw new InvalidOperationException($"Cannot complete interview with status {interview.Status}");

            interview.Status = InterviewStatus.Completed;
            interview.UpdatedAt = DateTime.UtcNow;
            interview.UpdatedBy = userId;

            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            // Update application status to Interviewed
            await _applicationService.UpdateStatusAsync(
                interview.JobApplicationId,
                ApplicationStatus.Interviewed,
                "Interview completed",
                userId
            );

            return true;
        }

        public async Task<bool> MarkInterviewNoShowAsync(int interviewId, int userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(interviewId);
            if (interview == null)
                return false;

            if (interview.Status != InterviewStatus.Scheduled && interview.Status != InterviewStatus.Rescheduled)
                throw new InvalidOperationException($"Cannot mark interview with status {interview.Status} as no-show");

            interview.Status = InterviewStatus.NoShow;
            interview.UpdatedAt = DateTime.UtcNow;
            interview.UpdatedBy = userId;

            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateCompanyAccessAsync(int interviewId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var userRoles = await _userRepository.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
                return true;

            var companyId = await _interviewRepository.GetCompanyIdByInterviewIdAsync(interviewId);
            if (!companyId.HasValue)
                return false;

            var userCompanyId = await _userRepository.GetUserCompanyIdAsync(userId);
            return userCompanyId == companyId;
        }

        public async Task<bool> CanUserModifyInterviewAsync(int interviewId, int userId)
        {
            return await ValidateCompanyAccessAsync(interviewId, userId);
        }
    }
}