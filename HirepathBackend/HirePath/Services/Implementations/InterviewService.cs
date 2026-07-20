using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationService _applicationService;

        public InterviewService(
            IInterviewRepository interviewRepo,
            IApplicationRepository appRepo,
            IUserRepository userRepo,
            IApplicationService applicationService)
        {
            _interviewRepo = interviewRepo;
            _appRepo = appRepo;
            _userRepo = userRepo;
            _applicationService = applicationService;
        }

        private async Task<int?> GetCompanyIdForUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user?.CompanyId;
        }

        private async Task<bool> HasCompanyAccessAsync(Job? job, int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            if (job == null)
                return false;

            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            return companyId != null && companyId == job.CompanyId;
        }

        public async Task<Interview> ScheduleAsync(Interview interview, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(interview.JobApplicationId);
            if (application == null)
                throw new KeyNotFoundException("Job application not found.");

            if (!await HasCompanyAccessAsync(application.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            interview.Status = InterviewStatus.Scheduled;
            interview.ScheduledByUserId = actingUserId;

            await _interviewRepo.AddAsync(interview);
            await _interviewRepo.SaveChangesAsync();

            // Auto-advance the application workflow, but only if it hasn't
            // already moved further along (e.g. don't downgrade "Hired").
            if (application.Status is ApplicationStatus.Applied
                or ApplicationStatus.UnderReview
                or ApplicationStatus.Shortlisted)
            {
                await _applicationService.UpdateStatusAsync(
                    application.Id,
                    ApplicationStatus.InterviewScheduled,
                    "Interview scheduled",
                    actingUserId,
                    isAdmin);
            }

            return interview;
        }

        public async Task<Interview?> GetByIdAsync(int id, int actingUserId, bool isAdmin)
        {
            var interview = await _interviewRepo.GetByIdWithCompanyAsync(id);
            if (interview?.JobApplication == null)
                return null;

            var isCandidateOwner = interview.JobApplication.CandidateProfile != null
                && interview.JobApplication.CandidateProfile.UserId == actingUserId;

            var hasCompanyAccess = await HasCompanyAccessAsync(interview.JobApplication.Job, actingUserId, isAdmin);

            return (isCandidateOwner || hasCompanyAccess) ? interview : null;
        }

        public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(applicationId);
            if (application == null)
                return Enumerable.Empty<Interview>();

            var isCandidateOwner = application.CandidateProfile != null
                && application.CandidateProfile.UserId == actingUserId;

            var hasCompanyAccess = await HasCompanyAccessAsync(application.Job, actingUserId, isAdmin);

            if (!isCandidateOwner && !hasCompanyAccess)
                return Enumerable.Empty<Interview>();

            return await _interviewRepo.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<Interview>> GetByCompanyAsync(int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return await _interviewRepo.GetAllAsync();

            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            if (companyId == null)
                return Enumerable.Empty<Interview>();

            return await _interviewRepo.GetByCompanyAsync(companyId.Value);
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
            var existing = await _interviewRepo.GetByIdWithCompanyAsync(interviewId);
            if (existing?.JobApplication == null)
                return false;

            if (!await HasCompanyAccessAsync(existing.JobApplication.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            // Only touch fields the caller actually supplied — everything
            // else keeps its current value instead of being reset to a default.
            if (scheduledAt.HasValue)
                existing.ScheduledAt = scheduledAt.Value;

            if (meetingLink != null)
                existing.MeetingLink = meetingLink;

            if (location != null)
                existing.Location = location;

            if (panel != null)
                existing.Panel = panel;

            if (notes != null)
                existing.Notes = notes;

            if (status.HasValue)
                existing.Status = status.Value;

            existing.UpdatedAt = DateTime.UtcNow;

            _interviewRepo.Update(existing);
            await _interviewRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelAsync(int id, string? notes, int actingUserId, bool isAdmin)
        {
            var existing = await _interviewRepo.GetByIdWithCompanyAsync(id);
            if (existing?.JobApplication == null)
                return false;

            if (!await HasCompanyAccessAsync(existing.JobApplication.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            existing.Status = InterviewStatus.Cancelled;
            existing.Notes = notes ?? existing.Notes;
            existing.UpdatedAt = DateTime.UtcNow;

            _interviewRepo.Update(existing);
            await _interviewRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interview = await _interviewRepo.GetByIdAsync(id);
            if (interview == null)
                return false;

            _interviewRepo.Delete(interview);
            await _interviewRepo.SaveChangesAsync();
            return true;
        }
    }
}