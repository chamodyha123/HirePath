using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _appRepo;
        private readonly IJobService _jobService;
        private readonly ICandidateService _candidateService;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationStatusHistoryRepository _historyRepo;

        private static readonly HashSet<ApplicationStatus> TerminalStatuses = new()
        {
            ApplicationStatus.Hired,
            ApplicationStatus.Rejected,
            ApplicationStatus.Withdrawn
        };

        public ApplicationService(
            IApplicationRepository appRepo,
            IJobService jobService,
            ICandidateService candidateService,
            IUserRepository userRepo,
            IApplicationStatusHistoryRepository historyRepo)
        {
            _appRepo = appRepo;
            _jobService = jobService;
            _candidateService = candidateService;
            _userRepo = userRepo;
            _historyRepo = historyRepo;
        }

        private async Task<int?> GetCompanyIdForUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user?.CompanyId;
        }

        private async Task<bool> HasCompanyAccessAsync(JobApplication app, int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            if (app.Job == null)
                return false;

            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            return companyId != null && companyId == app.Job.CompanyId;
        }

        private static bool IsCandidateOwner(JobApplication app, int actingUserId)
        {
            return app.CandidateProfile != null && app.CandidateProfile.UserId == actingUserId;
        }

        public async Task<JobApplication> ApplyAsync(int jobId, string? coverLetter, int? resumeId, int actingUserId)
        {
            var candidateProfile = await _candidateService.GetProfileAsync(actingUserId);
            if (candidateProfile == null)
                throw new InvalidOperationException("Create your candidate profile before applying for jobs.");

            var job = await _jobService.GetByIdAsync(jobId);
            if (job == null || !job.IsActive)
                throw new InvalidOperationException("This job is not open for applications.");

            var existing = await _appRepo.GetCandidateApplications(candidateProfile.Id);
            if (existing.Any(a => a.JobId == jobId && a.Status != ApplicationStatus.Withdrawn))
                throw new InvalidOperationException("You have already applied for this job.");

            int? resolvedResumeId;
            if (resumeId.HasValue)
            {
                var ownsResume = candidateProfile.Resumes.Any(r => r.Id == resumeId.Value);
                if (!ownsResume)
                    throw new InvalidOperationException("Selected resume does not belong to your profile.");

                resolvedResumeId = resumeId.Value;
            }
            else
            {
                resolvedResumeId = candidateProfile.Resumes.FirstOrDefault(r => r.IsPrimary)?.Id
                    ?? candidateProfile.Resumes.FirstOrDefault()?.Id;
            }

            if (resolvedResumeId == null)
                throw new InvalidOperationException("Upload a resume before applying for jobs.");

            var application = new JobApplication
            {
                JobId = jobId,
                CandidateProfileId = candidateProfile.Id,
                ResumeId = resolvedResumeId,
                CoverLetter = coverLetter,
                Status = ApplicationStatus.Applied,
                AppliedDate = DateTime.UtcNow
            };

            await _appRepo.AddAsync(application);
            await _appRepo.SaveChangesAsync();

            return application;
        }

        public async Task<bool> WithdrawAsync(int id, int actingUserId)
        {
            var app = await _appRepo.GetByIdWithDetailsAsync(id);
            if (app == null)
                return false;

            if (!IsCandidateOwner(app, actingUserId))
                return false;

            if (TerminalStatuses.Contains(app.Status))
                throw new InvalidOperationException($"This application is already {app.Status} and cannot be withdrawn.");

            var previousStatus = app.Status;
            app.Status = ApplicationStatus.Withdrawn;
            app.UpdatedAt = DateTime.UtcNow;

            _appRepo.Update(app);

            await _historyRepo.AddAsync(new ApplicationStatusHistory
            {
                JobApplicationId = app.Id,
                FromStatus = previousStatus,
                ToStatus = ApplicationStatus.Withdrawn,
                ChangedByUserId = actingUserId,
                Notes = "Withdrawn by candidate"
            });

            await _appRepo.SaveChangesAsync();
            return true;
        }

        public async Task<JobApplication?> GetByIdAsync(int id, int actingUserId, bool isAdmin)
        {
            var app = await _appRepo.GetByIdWithDetailsAsync(id);
            if (app == null)
                return null;

            var allowed = IsCandidateOwner(app, actingUserId) || await HasCompanyAccessAsync(app, actingUserId, isAdmin);

            return allowed ? app : null;
        }

        public async Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(int actingUserId)
        {
            var candidateProfile = await _candidateService.GetProfileAsync(actingUserId);
            if (candidateProfile == null)
                return Enumerable.Empty<JobApplication>();

            return await _appRepo.GetCandidateApplications(candidateProfile.Id);
        }

        public async Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateProfileId, int actingUserId, bool isAdmin)
        {
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only platform admins can view another candidate's applications.");

            return await _appRepo.GetCandidateApplications(candidateProfileId);
        }

        public async Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId, int actingUserId, bool isAdmin)
        {
            var job = await _jobService.GetByIdAsync(jobId);
            if (job == null)
                return Enumerable.Empty<JobApplication>();

            if (!isAdmin)
            {
                var companyId = await GetCompanyIdForUserAsync(actingUserId);
                if (companyId == null || companyId != job.CompanyId)
                    throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");
            }

            return await _appRepo.GetJobApplications(jobId);
        }

        public async Task<IEnumerable<JobApplication>> GetByCompanyAsync(int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return await _appRepo.GetAllAsync();

            var companyId = await GetCompanyIdForUserAsync(actingUserId);
            if (companyId == null)
                return Enumerable.Empty<JobApplication>();

            return await _appRepo.GetByCompanyAsync(companyId.Value);
        }

        private async Task<bool> TransitionAsync(int id, ApplicationStatus newStatus, string? notes, int actingUserId, bool isAdmin)
        {
            var app = await _appRepo.GetByIdWithDetailsAsync(id);
            if (app == null)
                return false;

            if (!await HasCompanyAccessAsync(app, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            if (TerminalStatuses.Contains(app.Status))
                throw new InvalidOperationException($"This application is already {app.Status} and cannot be changed further.");

            var previousStatus = app.Status;
            app.Status = newStatus;
            app.UpdatedAt = DateTime.UtcNow;

            _appRepo.Update(app);

            await _historyRepo.AddAsync(new ApplicationStatusHistory
            {
                JobApplicationId = app.Id,
                FromStatus = previousStatus,
                ToStatus = newStatus,
                ChangedByUserId = actingUserId,
                Notes = notes
            });

            await _appRepo.SaveChangesAsync();
            return true;
        }

        public Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, status, notes, actingUserId, isAdmin);

        public Task<bool> ShortlistAsync(int id, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, ApplicationStatus.Shortlisted, notes, actingUserId, isAdmin);

        public Task<bool> RejectAsync(int id, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, ApplicationStatus.Rejected, notes, actingUserId, isAdmin);

        public Task<bool> MarkInterviewCompletedAsync(int id, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, ApplicationStatus.Interviewed, notes, actingUserId, isAdmin);

        public Task<bool> SendOfferAsync(int id, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, ApplicationStatus.Offered, notes, actingUserId, isAdmin);

        public Task<bool> MarkHiredAsync(int id, string? notes, int actingUserId, bool isAdmin)
            => TransitionAsync(id, ApplicationStatus.Hired, notes, actingUserId, isAdmin);

        public async Task<bool> AddRecruiterNotesAsync(int id, string notes, int actingUserId, bool isAdmin)
        {
            var app = await _appRepo.GetByIdWithDetailsAsync(id);
            if (app == null)
                return false;

            if (!await HasCompanyAccessAsync(app, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            app.RecruiterNotes = notes;
            app.UpdatedAt = DateTime.UtcNow;

            _appRepo.Update(app);
            await _appRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var app = await _appRepo.GetByIdAsync(id);
            if (app == null)
                return false;

            _appRepo.Delete(app);
            await _appRepo.SaveChangesAsync();
            return true;
        }
    }
}