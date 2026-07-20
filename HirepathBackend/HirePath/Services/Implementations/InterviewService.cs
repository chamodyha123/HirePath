using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationService _applicationService;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(
            IInterviewRepository interviewRepo,
            IApplicationRepository appRepo,
            IUserRepository userRepo,
            IApplicationService applicationService,
            ILogger<InterviewService> logger)
        {
            _interviewRepo = interviewRepo;
            _appRepo = appRepo;
            _userRepo = userRepo;
            _applicationService = applicationService;
            _logger = logger;
        }

        private async Task<int?> GetCompanyIdForUserAsync(int userId)
        {
            return (await _userRepo.GetByIdAsync(userId))?.CompanyId;
        }

        private async Task<bool> HasCompanyAccessAsync(Job? job, int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            if (job == null)
                return false;

            var companyId = await GetCompanyIdForUserAsync(actingUserId);

            return companyId.HasValue &&
                   companyId.Value == job.CompanyId;
        }

        private static void ValidateDetails(Interview interview)
        {
            if (interview.ScheduledAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Interview date and time must be in the future.");

            if (interview.InterviewType == InterviewType.Online &&
                string.IsNullOrWhiteSpace(interview.MeetingLink))
            {
                throw new InvalidOperationException(
                    "A meeting link is required for an online interview.");
            }

            if (interview.InterviewType == InterviewType.Physical &&
                string.IsNullOrWhiteSpace(interview.Location))
            {
                throw new InvalidOperationException(
                    "A location is required for a physical interview.");
            }
        }

        public async Task<Interview> ScheduleAsync(
            Interview interview,
            int actingUserId,
            bool isAdmin)
        {
            ValidateDetails(interview);

            var application =
                await _appRepo.GetByIdWithDetailsAsync(interview.JobApplicationId)
                ?? throw new KeyNotFoundException("Job application not found.");

            if (!await HasCompanyAccessAsync(application.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException(
                    "You do not have access to this company's recruitment data.");

            if (application.Status != ApplicationStatus.Shortlisted)
                throw new InvalidOperationException(
                    "Only shortlisted applications can be scheduled for an interview.");

            var existing =
                await _interviewRepo.GetByApplicationIdAsync(application.Id);

            if (existing.Any(i =>
                    i.Status == InterviewStatus.Scheduled ||
                    i.Status == InterviewStatus.Rescheduled))
            {
                throw new InvalidOperationException(
                    "This application already has an active interview.");
            }

            interview.Status = InterviewStatus.Scheduled;
            interview.ScheduledByUserId = actingUserId;
            interview.MeetingLink = string.IsNullOrWhiteSpace(interview.MeetingLink)
                ? null
                : interview.MeetingLink.Trim();
            interview.Location = string.IsNullOrWhiteSpace(interview.Location)
                ? null
                : interview.Location.Trim();

            await _interviewRepo.AddAsync(interview);
            await _interviewRepo.SaveChangesAsync();

            await _applicationService.UpdateStatusAsync(
                application.Id,
                ApplicationStatus.InterviewScheduled,
                "Interview scheduled",
                actingUserId,
                isAdmin);

            _logger.LogInformation(
                "Interview {InterviewId} scheduled for application {ApplicationId} by user {UserId}",
                interview.Id,
                application.Id,
                actingUserId);

            return interview;
        }

        public async Task<Interview?> GetByIdAsync(
            int id,
            int actingUserId,
            bool isAdmin)
        {
            var interview =
                await _interviewRepo.GetByIdWithCompanyAsync(id);

            if (interview?.JobApplication == null)
                return null;

            var candidateOwner =
                interview.JobApplication.CandidateProfile?.UserId == actingUserId;

            return candidateOwner ||
                   await HasCompanyAccessAsync(
                       interview.JobApplication.Job,
                       actingUserId,
                       isAdmin)
                ? interview
                : null;
        }

        public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(
            int applicationId,
            int actingUserId,
            bool isAdmin)
        {
            var application =
                await _appRepo.GetByIdWithDetailsAsync(applicationId);

            if (application == null)
                return Enumerable.Empty<Interview>();

            var candidateOwner =
                application.CandidateProfile?.UserId == actingUserId;

            if (!candidateOwner &&
                !await HasCompanyAccessAsync(
                    application.Job,
                    actingUserId,
                    isAdmin))
            {
                return Enumerable.Empty<Interview>();
            }

            return await _interviewRepo.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<Interview>> GetByCompanyAsync(
            int actingUserId,
            bool isAdmin)
        {
            if (isAdmin)
                return await _interviewRepo.GetAllAsync();

            var companyId =
                await GetCompanyIdForUserAsync(actingUserId);

            return companyId.HasValue
                ? await _interviewRepo.GetByCompanyAsync(companyId.Value)
                : Enumerable.Empty<Interview>();
        }

        public async Task<bool> UpdateAsync(
            int interviewId,
            DateTime? scheduledAt,
            string? meetingLink,
            string? location,
            string? panel,
            string? notes,
            InterviewStatus? status,
            int actingUserId,
            bool isAdmin)
        {
            var existing =
                await _interviewRepo.GetByIdWithCompanyAsync(interviewId);

            if (existing?.JobApplication == null)
                return false;

            if (!await HasCompanyAccessAsync(
                    existing.JobApplication.Job,
                    actingUserId,
                    isAdmin))
            {
                throw new UnauthorizedAccessException(
                    "You do not have access to this company's recruitment data.");
            }

            if (existing.Status is InterviewStatus.Completed
                or InterviewStatus.Cancelled
                or InterviewStatus.NoShow)
            {
                throw new InvalidOperationException(
                    $"A {existing.Status} interview cannot be modified.");
            }

            if (scheduledAt.HasValue)
            {
                if (scheduledAt.Value <= DateTime.UtcNow)
                    throw new InvalidOperationException(
                        "Interview date and time must be in the future.");

                existing.ScheduledAt = scheduledAt.Value;

                if (existing.Status == InterviewStatus.Scheduled)
                    existing.Status = InterviewStatus.Rescheduled;
            }

            if (meetingLink != null)
                existing.MeetingLink = string.IsNullOrWhiteSpace(meetingLink)
                    ? null
                    : meetingLink.Trim();

            if (location != null)
                existing.Location = string.IsNullOrWhiteSpace(location)
                    ? null
                    : location.Trim();

            if (panel != null)
                existing.Panel = panel.Trim();

            if (notes != null)
                existing.Notes = notes.Trim();

            if (status.HasValue)
            {
                if (status.Value == InterviewStatus.Completed)
                    throw new InvalidOperationException(
                        "Submit interview feedback to complete an interview.");

                if (status.Value == InterviewStatus.Cancelled)
                {
                    return await CancelAsync(
                        interviewId,
                        notes,
                        actingUserId,
                        isAdmin);
                }

                existing.Status = status.Value;
            }

            ValidateDetails(existing);

            existing.UpdatedAt = DateTime.UtcNow;

            _interviewRepo.Update(existing);
            await _interviewRepo.SaveChangesAsync();

            _logger.LogInformation(
                "Interview {InterviewId} updated by user {UserId}",
                interviewId,
                actingUserId);

            return true;
        }

        public async Task<bool> CancelAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin)
        {
            var existing =
                await _interviewRepo.GetByIdWithCompanyAsync(id);

            if (existing?.JobApplication == null)
                return false;

            if (!await HasCompanyAccessAsync(
                    existing.JobApplication.Job,
                    actingUserId,
                    isAdmin))
            {
                throw new UnauthorizedAccessException(
                    "You do not have access to this company's recruitment data.");
            }

            if (existing.Status == InterviewStatus.Completed)
                throw new InvalidOperationException(
                    "A completed interview cannot be cancelled.");

            if (existing.Status == InterviewStatus.Cancelled)
                throw new InvalidOperationException(
                    "Interview is already cancelled.");

            existing.Status = InterviewStatus.Cancelled;
            existing.Notes = string.IsNullOrWhiteSpace(notes)
                ? existing.Notes
                : notes.Trim();
            existing.UpdatedAt = DateTime.UtcNow;

            _interviewRepo.Update(existing);
            await _interviewRepo.SaveChangesAsync();

            _logger.LogInformation(
                "Interview {InterviewId} cancelled by user {UserId}",
                id,
                actingUserId);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interview = await _interviewRepo.GetByIdAsync(id);

            if (interview == null)
                return false;

            _interviewRepo.Delete(interview);
            await _interviewRepo.SaveChangesAsync();

            _logger.LogWarning(
                "Interview {InterviewId} permanently deleted by an administrator",
                id);

            return true;
        }
    }
}