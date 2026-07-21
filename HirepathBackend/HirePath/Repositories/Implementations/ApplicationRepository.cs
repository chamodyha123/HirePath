using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class ApplicationRepository
        : GenericRepository<JobApplication>,
          IApplicationRepository
    {
        public ApplicationRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        // ============ EXISTING METHODS ============

        public async Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId)
        {
            return await _context.JobApplications
                .Where(x => x.CandidateProfileId == candidateId)
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.Job)
                .ThenInclude(j => j.Department)
                .Include(x => x.CandidateProfile)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetJobApplications(int jobId)
        {
            return await _context.JobApplications
                .Where(x => x.JobId == jobId)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Skills)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Resumes)
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.Interviews)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByCompanyAsync(int companyId)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.Job)
                .ThenInclude(j => j.Department)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Skills)
                .Include(x => x.Interviews)
                .Where(x => x.Job != null && x.Job.CompanyId == companyId)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        // ============ NEW METHODS ============

        public async Task<JobApplication?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.Job)
                .ThenInclude(j => j.Department)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Skills)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Educations)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Experiences)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.Resumes)
                .Include(x => x.Interviews)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CandidateProfile?> GetCandidateProfileByApplicationId(int applicationId)
        {
            var application = await _context.JobApplications
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(x => x.Id == applicationId);

            return application?.CandidateProfile;
        }

        public async Task<int?> GetCompanyIdByApplicationIdAsync(int applicationId)
        {
            var application = await _context.JobApplications
                .Include(x => x.Job)
                .FirstOrDefaultAsync(x => x.Id == applicationId);

            return application?.Job?.CompanyId;
        }

        public async Task<bool> UpdateStatusAsync(int applicationId, ApplicationStatus status, string? notes, int userId)
        {
            var application = await _context.JobApplications.FindAsync(applicationId);
            if (application == null)
                return false;

            var oldStatus = application.Status;
            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            // Add status history
            await AddStatusHistoryAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                Status = status,
                Notes = notes ?? $"Status changed from {oldStatus} to {status}",
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddStatusHistoryAsync(ApplicationStatusHistory history)
        {
            await _context.ApplicationStatusHistories.AddAsync(history);
        }

        public async Task<IEnumerable<ApplicationStatusHistory>> GetStatusHistoryAsync(int applicationId)
        {
            return await _context.ApplicationStatusHistories
                .Where(x => x.ApplicationId == applicationId)
                .Include(x => x.ChangedByUser)
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(int companyId, ApplicationStatus status)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.Interviews)
                .Where(x => x.Job != null && x.Job.CompanyId == companyId && x.Status == status)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.Interviews)
                .Where(x => x.Job != null &&
                           x.Job.CompanyId == companyId &&
                           x.AppliedDate >= startDate &&
                           x.AppliedDate <= endDate)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsWithInterviewsAsync(int companyId)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.Interviews)
                .Where(x => x.Job != null &&
                           x.Job.CompanyId == companyId &&
                           x.Interviews.Any())
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }

        public async Task<int> GetApplicationCountByCompanyAsync(int companyId)
        {
            return await _context.JobApplications
                .Where(x => x.Job != null && x.Job.CompanyId == companyId)
                .CountAsync();
        }

        public async Task<Dictionary<ApplicationStatus, int>> GetApplicationStatsByCompanyAsync(int companyId)
        {
            var applications = await _context.JobApplications
                .Where(x => x.Job != null && x.Job.CompanyId == companyId)
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return applications.ToDictionary(x => x.Status, x => x.Count);
        }

        public async Task<bool> HasApplicationAsync(int jobId, int candidateProfileId)
        {
            return await _context.JobApplications
                .AnyAsync(x => x.JobId == jobId &&
                              x.CandidateProfileId == candidateProfileId &&
                              x.Status != ApplicationStatus.Withdrawn);
        }

        public async Task<IEnumerable<JobApplication>> GetWithdrawnApplicationsAsync(int candidateProfileId)
        {
            return await _context.JobApplications
                .Where(x => x.CandidateProfileId == candidateProfileId &&
                           x.Status == ApplicationStatus.Withdrawn)
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetRecentApplicationsByCompanyAsync(int companyId, int count)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Where(x => x.Job != null && x.Job.CompanyId == companyId)
                .OrderByDescending(x => x.AppliedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsForRecruiterAsync(int recruiterId)
        {
            // Get the recruiter's company
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == recruiterId);

            if (user?.Company == null)
                return Enumerable.Empty<JobApplication>();

            return await GetApplicationsByCompanyAsync(user.CompanyId.Value);
        }

        // ============ OVERRIDES ============

        public override async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.CandidateProfile)
                .Include(x => x.Interviews)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<JobApplication>> GetAllAsync()
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .ThenInclude(j => j.Company)
                .Include(x => x.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Include(x => x.Interviews)
                .OrderByDescending(x => x.AppliedDate)
                .ToListAsync();
        }
    }
}