using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HirePathAI.API.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _appRepo;
        private readonly IJobService _jobService;
        private readonly ICandidateService _candidateService;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationStatusHistoryRepository _historyRepo;
        private readonly ILogger<ApplicationService> _logger;

        private static readonly HashSet<ApplicationStatus> TerminalStatuses = new()
        {
            ApplicationStatus.Hired,
            ApplicationStatus.Rejected,
            ApplicationStatus.Withdrawn
        };

        private static readonly IReadOnlyDictionary<ApplicationStatus, HashSet<ApplicationStatus>> AllowedTransitions =
            new Dictionary<ApplicationStatus, HashSet<ApplicationStatus>>
            {
                [ApplicationStatus.Applied] = new() { ApplicationStatus.UnderReview, ApplicationStatus.Shortlisted, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn },
                [ApplicationStatus.UnderReview] = new() { ApplicationStatus.Shortlisted, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn },
                [ApplicationStatus.Shortlisted] = new() { ApplicationStatus.InterviewScheduled, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn },
                [ApplicationStatus.InterviewScheduled] = new() { ApplicationStatus.Interviewed, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn },
                [ApplicationStatus.Interviewed] = new() { ApplicationStatus.Offered, ApplicationStatus.Rejected },
                [ApplicationStatus.Offered] = new() { ApplicationStatus.Hired, ApplicationStatus.Rejected }
            };

        public ApplicationService(
            IApplicationRepository appRepo,
            IJobService jobService,
            ICandidateService candidateService,
            IUserRepository userRepo,
            IApplicationStatusHistoryRepository historyRepo,
            ILogger<ApplicationService> logger)
        {
            _appRepo = appRepo;
            _jobService = jobService;
            _candidateService = candidateService;
            _userRepo = userRepo;
            _historyRepo = historyRepo;
            _logger = logger;
        }

        private async Task<int?> GetCompanyIdForUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user?.CompanyId;
        }

        private async Task<bool> HasCompanyAccessAsync(JobApplication application, int actingUserId, bool isAdmin)
        {
            if (isAdmin) return true;
            if (application.Job == null) return false;
            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            return companyId.HasValue && companyId.Value == application.Job.CompanyId;
        }

        private static bool IsCandidateOwner(JobApplication application, int actingUserId) =>
            application.CandidateProfile?.UserId == actingUserId;

        private static void ValidateTransition(ApplicationStatus currentStatus, ApplicationStatus newStatus)
        {
            if (currentStatus == newStatus)
                throw new InvalidOperationException($"Application is already in {currentStatus} status.");

            if (TerminalStatuses.Contains(currentStatus))
                throw new InvalidOperationException($"This application is already {currentStatus} and cannot be changed further.");

            if (!AllowedTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatus))
                throw new InvalidOperationException($"Invalid workflow transition from {currentStatus} to {newStatus}.");
        }

        public async Task<JobApplication> ApplyAsync(int jobId, string? coverLetter, int? resumeId, int actingUserId)
        {
            if (jobId <= 0) throw new ArgumentException("A valid job ID is required.", nameof(jobId));

            var candidateProfile = await _candidateService.GetProfileAsync(actingUserId)
                ?? throw new InvalidOperationException("Create your candidate profile before applying for jobs.");

            var job = await _jobService.GetByIdAsync(jobId);
            if (job == null || !job.IsActive)
                throw new InvalidOperationException("This job is not open for applications.");

            var existing = await _appRepo.GetCandidateApplications(candidateProfile.Id);
            if (existing.Any(a => a.JobId == jobId && a.Status != ApplicationStatus.Withdrawn))
                throw new InvalidOperationException("You have already applied for this job.");

            int? resolvedResumeId;
            if (resumeId.HasValue)
            {
                if (!candidateProfile.Resumes.Any(r => r.Id == resumeId.Value))
                    throw new InvalidOperationException("Selected resume does not belong to your profile.");
                resolvedResumeId = resumeId.Value;
            }
            else
            {
                resolvedResumeId = candidateProfile.Resumes.FirstOrDefault(r => r.IsPrimary)?.Id
                    ?? candidateProfile.Resumes.FirstOrDefault()?.Id;
            }

            if (!resolvedResumeId.HasValue)
                throw new InvalidOperationException("Upload a resume before applying for jobs.");

            var application = new JobApplication
            {
                JobId = jobId,
                CandidateProfileId = candidateProfile.Id,
                ResumeId = resolvedResumeId,
                CoverLetter = string.IsNullOrWhiteSpace(coverLetter) ? null : coverLetter.Trim(),
                Status = ApplicationStatus.Applied,
                AppliedDate = DateTime.UtcNow
            };

            await _appRepo.AddAsync(application);
            await _appRepo.SaveChangesAsync();

            await _historyRepo.AddAsync(new ApplicationStatusHistory
            {
                JobApplicationId = application.Id,
                FromStatus = ApplicationStatus.Applied,
                ToStatus = ApplicationStatus.Applied,
                ChangedByUserId = actingUserId,
                Notes = "Application submitted"
            });
            await _historyRepo.SaveChangesAsync();

            _logger.LogInformation("Candidate user {UserId} submitted application {ApplicationId} for job {JobId}", actingUserId, application.Id, jobId);
            return application;
        }

        public async Task<bool> WithdrawAsync(int id, int actingUserId)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(id);
            if (application == null || !IsCandidateOwner(application, actingUserId)) return false;
            return await TransitionAsync(id, ApplicationStatus.Withdrawn, "Withdrawn by candidate", actingUserId, false, allowCandidateOwner: true);
        }

        public async Task<JobApplication?> GetByIdAsync(int id, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(id);
            if (application == null) return null;
            return IsCandidateOwner(application, actingUserId) || await HasCompanyAccessAsync(application, actingUserId, isAdmin)
                ? application : null;
        }

        public async Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(int actingUserId)
        {
            var profile = await _candidateService.GetProfileAsync(actingUserId);
            return profile == null ? Enumerable.Empty<JobApplication>() : await _appRepo.GetCandidateApplications(profile.Id);
        }

        public async Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateProfileId, int actingUserId, bool isAdmin)
        {
            if (!isAdmin) throw new UnauthorizedAccessException("Only platform admins can view another candidate's applications.");
            return await _appRepo.GetCandidateApplications(candidateProfileId);
        }

        public async Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId, int actingUserId, bool isAdmin)
        {
            var job = await _jobService.GetByIdAsync(jobId);
            if (job == null) return Enumerable.Empty<JobApplication>();
            if (!isAdmin)
            {
                var companyId = await GetCompanyIdForUserAsync(actingUserId);
                if (!companyId.HasValue || companyId.Value != job.CompanyId)
                    throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");
            }
            return await _appRepo.GetJobApplications(jobId);
        }

        public async Task<IEnumerable<JobApplication>> GetByCompanyAsync(int actingUserId, bool isAdmin)
        {
            if (isAdmin) return await _appRepo.GetAllAsync();
            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            return companyId.HasValue ? await _appRepo.GetByCompanyAsync(companyId.Value) : Enumerable.Empty<JobApplication>();
        }

        public async Task<IEnumerable<ApplicationStatusHistory>> GetHistoryAsync(int applicationId, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(applicationId);
            if (application == null) return Enumerable.Empty<ApplicationStatusHistory>();
            if (!IsCandidateOwner(application, actingUserId) && !await HasCompanyAccessAsync(application, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this application history.");
            return await _historyRepo.GetByJobApplicationIdAsync(applicationId);
        }

        private async Task<bool> TransitionAsync(int id, ApplicationStatus newStatus, string? notes, int actingUserId, bool isAdmin, bool allowCandidateOwner = false)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(id);
            if (application == null) return false;

            var authorized = allowCandidateOwner && IsCandidateOwner(application, actingUserId)
                || await HasCompanyAccessAsync(application, actingUserId, isAdmin);
            if (!authorized)
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            ValidateTransition(application.Status, newStatus);
            var previousStatus = application.Status;
            application.Status = newStatus;
            application.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(notes)) application.CompanyFeedback = notes.Trim();

            _appRepo.Update(application);
            await _historyRepo.AddAsync(new ApplicationStatusHistory
            {
                JobApplicationId = application.Id,
                FromStatus = previousStatus,
                ToStatus = newStatus,
                ChangedByUserId = actingUserId,
                Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
            });
            await _appRepo.SaveChangesAsync();

            _logger.LogInformation("Application {ApplicationId} changed from {FromStatus} to {ToStatus} by user {UserId}", id, previousStatus, newStatus, actingUserId);
            return true;
        }

        public Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, status, notes, actingUserId, isAdmin);
        public Task<bool> ShortlistAsync(int id, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, ApplicationStatus.Shortlisted, notes, actingUserId, isAdmin);
        public Task<bool> RejectAsync(int id, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, ApplicationStatus.Rejected, notes, actingUserId, isAdmin);
        public Task<bool> MarkInterviewCompletedAsync(int id, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, ApplicationStatus.Interviewed, notes, actingUserId, isAdmin);
        public Task<bool> SendOfferAsync(int id, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, ApplicationStatus.Offered, notes, actingUserId, isAdmin);
        public Task<bool> MarkHiredAsync(int id, string? notes, int actingUserId, bool isAdmin) => TransitionAsync(id, ApplicationStatus.Hired, notes, actingUserId, isAdmin);

        public async Task<bool> AddRecruiterNotesAsync(int id, string notes, int actingUserId, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(notes)) throw new ArgumentException("Notes cannot be empty.", nameof(notes));
            var application = await _appRepo.GetByIdWithDetailsAsync(id);
            if (application == null) return false;
            if (!await HasCompanyAccessAsync(application, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");
            application.RecruiterNotes = notes.Trim();
            application.UpdatedAt = DateTime.UtcNow;
            _appRepo.Update(application);
            await _appRepo.SaveChangesAsync();
            _logger.LogInformation("Recruiter notes updated for application {ApplicationId} by user {UserId}", id, actingUserId);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await _appRepo.GetByIdAsync(id);
            if (application == null) return false;
            _appRepo.Delete(application);
            await _appRepo.SaveChangesAsync();
            _logger.LogWarning("Application {ApplicationId} permanently deleted by an administrator", id);
            return true;
        }
    }
}
