using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HirePathAI.API.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IInterviewRepository interviewRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _interviewRepository = interviewRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        private async Task<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");
            return user;
        }

        // ============ APPLICATION CRUD ============

        public async Task<JobApplication> ApplyAsync(CreateApplicationDto dto, int userId)
        {
            // Check if job exists and is active
            var job = await _jobRepository.GetByIdAsync(dto.JobId);
            if (job == null)
                throw new ArgumentException("Job not found");
            if (!job.IsActive)
                throw new InvalidOperationException("This job is no longer active");

            // Check if candidate already applied
            var existingApplications = await _applicationRepository.GetCandidateApplications(dto.CandidateProfileId);
            if (existingApplications.Any(a => a.JobId == dto.JobId && a.Status != ApplicationStatus.Withdrawn))
                throw new InvalidOperationException("You have already applied for this job");

            var application = new JobApplication
            {
                JobId = dto.JobId,
                CandidateProfileId = dto.CandidateProfileId,
                CoverLetter = dto.CoverLetter,
                Status = ApplicationStatus.Applied,
                AppliedDate = DateTime.UtcNow
            };

            await _applicationRepository.AddAsync(application);
            await _applicationRepository.SaveChangesAsync();

            // Create status history entry
            await AddStatusHistoryAsync(application.Id, ApplicationStatus.Applied, "Application submitted", userId);

            return application;
        }

        public async Task<JobApplication?> GetApplicationByIdAsync(int id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByCandidateAsync(int candidateProfileId)
        {
            return await _applicationRepository.GetCandidateApplications(candidateProfileId);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByJobAsync(int jobId)
        {
            return await _applicationRepository.GetJobApplications(jobId);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByCompanyAsync(int companyId)
        {
            return await _applicationRepository.GetApplicationsByCompanyAsync(companyId);
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, ApplicationStatus status, string? notes, int userId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return false;

            // Validate status transition
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null)
                return false;

            var roles = await _userRepository.GetRolesAsync(currentUser) ?? Enumerable.Empty<string>();
            var userRole = roles.FirstOrDefault() ?? "Candidate";

            if (!CanTransitionTo(application.Status, status, userRole))
                throw new InvalidOperationException($"Cannot transition from {application.Status} to {status} with role {userRole}");

            var oldStatus = application.Status;
            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            await _applicationRepository.UpdateAsync(application);
            await _applicationRepository.SaveChangesAsync();

            // Add status history
            await AddStatusHistoryAsync(applicationId, status, notes ?? $"Status changed from {oldStatus} to {status}", userId);

            return true;
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return false;

            await _applicationRepository.DeleteAsync(application);
            await _applicationRepository.SaveChangesAsync();
            return true;
        }

        // ============ WORKFLOW ACTIONS ============

        public async Task<bool> ShortlistAsync(int applicationId, string? notes, int userId)
        {
            return await UpdateStatusAsync(applicationId, ApplicationStatus.Shortlisted, notes ?? "Candidate shortlisted for interview", userId);
        }

        public async Task<bool> RejectAsync(int applicationId, string? notes, int userId)
        {
            return await UpdateStatusAsync(applicationId, ApplicationStatus.Rejected, notes ?? "Application rejected", userId);
        }

        public async Task<bool> ScheduleInterviewAsync(int applicationId, WorkflowActionDto dto, int userId)
        {
            // First update status to InterviewScheduled
            var statusUpdated = await UpdateStatusAsync(
                applicationId,
                ApplicationStatus.InterviewScheduled,
                $"Interview scheduled for {dto.ScheduledDate?.ToString("yyyy-MM-dd HH:mm")}",
                userId
            );

            if (!statusUpdated)
                return false;

            // Create interview record
            var interview = new Interview
            {
                JobApplicationId = applicationId,
                ScheduledAt = dto.ScheduledDate ?? DateTime.UtcNow.AddDays(3),
                InterviewType = dto.InterviewType ?? InterviewType.Online,
                MeetingLink = dto.MeetingLink ?? string.Empty,
                Status = InterviewStatus.Scheduled,
                Notes = dto.Notes
            };

            await _interviewRepository.AddAsync(interview);
            await _interviewRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SendOfferAsync(int applicationId, WorkflowActionDto dto, int userId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return false;

            // Validate that status is Interviewed or Shortlisted
            if (application.Status != ApplicationStatus.Interviewed && application.Status != ApplicationStatus.Shortlisted)
                throw new InvalidOperationException($"Cannot send offer for application in {application.Status} status");

            return await UpdateStatusAsync(
                applicationId,
                ApplicationStatus.Offered,
                $"Offer sent: {dto.OfferDetails} (Salary: {dto.OfferSalary?.ToString("C")})",
                userId
            );
        }

        public async Task<bool> HireAsync(int applicationId, string? notes, int userId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return false;

            if (application.Status != ApplicationStatus.Offered)
                throw new InvalidOperationException($"Cannot hire candidate in {application.Status} status. Must be Offered first.");

            return await UpdateStatusAsync(
                applicationId,
                ApplicationStatus.Hired,
                notes ?? "Candidate hired successfully",
                userId
            );
        }

        public async Task<bool> WithdrawApplicationAsync(int applicationId, int userId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return false;

            // Only the candidate who applied can withdraw
            var candidateProfile = await _applicationRepository.GetCandidateProfileByApplicationId(applicationId);
            if (candidateProfile == null || candidateProfile.UserId != userId)
                throw new UnauthorizedAccessException("You can only withdraw your own applications");

            if (application.Status == ApplicationStatus.Hired || application.Status == ApplicationStatus.Rejected)
                throw new InvalidOperationException($"Cannot withdraw application with status {application.Status}");

            return await UpdateStatusAsync(
                applicationId,
                ApplicationStatus.Withdrawn,
                "Application withdrawn by candidate",
                userId
            );
        }

        // ============ STATUS TRANSITIONS ============

        public bool CanTransitionTo(ApplicationStatus currentStatus, ApplicationStatus newStatus, string userRole)
        {
            // Admin can do anything
            if (userRole == "Admin")
                return true;

            // Candidate can only withdraw or apply
            if (userRole == "Candidate")
            {
                return currentStatus == ApplicationStatus.Applied && newStatus == ApplicationStatus.Withdrawn;
            }

            // Recruiter can: UnderReview, Shortlist, Reject, Schedule Interview
            if (userRole == "Recruiter")
            {
                return (currentStatus == ApplicationStatus.Applied && newStatus == ApplicationStatus.UnderReview) ||
                       (currentStatus == ApplicationStatus.UnderReview && newStatus == ApplicationStatus.Shortlisted) ||
                       (currentStatus == ApplicationStatus.UnderReview && newStatus == ApplicationStatus.Rejected) ||
                       (currentStatus == ApplicationStatus.Shortlisted && newStatus == ApplicationStatus.InterviewScheduled) ||
                       (currentStatus == ApplicationStatus.Applied && newStatus == ApplicationStatus.Rejected);
            }

            // Hiring Manager can: Interviewed, Offer, Hire
            if (userRole == "HiringManager")
            {
                return (currentStatus == ApplicationStatus.InterviewScheduled && newStatus == ApplicationStatus.Interviewed) ||
                       (currentStatus == ApplicationStatus.Interviewed && newStatus == ApplicationStatus.Offered) ||
                       (currentStatus == ApplicationStatus.Offered && newStatus == ApplicationStatus.Hired) ||
                       (currentStatus == ApplicationStatus.Interviewed && newStatus == ApplicationStatus.Rejected);
            }

            return false;
        }

        public async Task<IEnumerable<ApplicationStatusHistory>> GetStatusHistoryAsync(int applicationId)
        {
            return await _applicationRepository.GetStatusHistoryAsync(applicationId);
        }

        public async Task<bool> ValidateCompanyAccessAsync(int applicationId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var userRoles = await _userRepository.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
                return true;

            if (userRoles.Contains("Candidate"))
            {
                // Candidate can only access their own applications
                var application = await _applicationRepository.GetByIdAsync(applicationId);
                if (application == null)
                    return false;

                var candidateProfile = await _applicationRepository.GetCandidateProfileByApplicationId(applicationId);
                return candidateProfile != null && candidateProfile.UserId == userId;
            }

            // Recruiter and HiringManager - check company access
            var companyId = await _applicationRepository.GetCompanyIdByApplicationIdAsync(applicationId);
            if (!companyId.HasValue)
                return false;

            var userEntity = await _userRepository.GetByIdAsync(userId);
            var userCompanyId = userEntity?.CompanyId;
            return userCompanyId == companyId;
        }

        // ============ PRIVATE HELPERS ============

        private async Task AddStatusHistoryAsync(int applicationId, ApplicationStatus status, string notes, int userId)
        {
            var history = new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                Status = status,
                Notes = notes,
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow
            };

            await _applicationRepository.AddStatusHistoryAsync(history);
            await _applicationRepository.SaveChangesAsync();
        }
    }
}