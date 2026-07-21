using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IApplicationRepository : IGenericRepository<JobApplication>
    {
        // ============ EXISTING METHODS ============
        Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId);
        Task<IEnumerable<JobApplication>> GetJobApplications(int jobId);
        Task<IEnumerable<JobApplication>> GetApplicationsByCompanyAsync(int companyId);

        // ============ NEW METHODS ============
        Task<JobApplication?> GetByIdWithDetailsAsync(int id);
        Task<CandidateProfile?> GetCandidateProfileByApplicationId(int applicationId);
        Task<int?> GetCompanyIdByApplicationIdAsync(int applicationId);
        Task<bool> UpdateStatusAsync(int applicationId, ApplicationStatus status, string? notes, int userId);
        Task AddStatusHistoryAsync(ApplicationStatusHistory history);
        Task<IEnumerable<ApplicationStatusHistory>> GetStatusHistoryAsync(int applicationId);
        Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(int companyId, ApplicationStatus status);
        Task<IEnumerable<JobApplication>> GetApplicationsByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<JobApplication>> GetApplicationsWithInterviewsAsync(int companyId);
        Task<int> GetApplicationCountByCompanyAsync(int companyId);
        Task<Dictionary<ApplicationStatus, int>> GetApplicationStatsByCompanyAsync(int companyId);
        Task<bool> HasApplicationAsync(int jobId, int candidateProfileId);
        Task<IEnumerable<JobApplication>> GetWithdrawnApplicationsAsync(int candidateProfileId);
        Task<IEnumerable<JobApplication>> GetRecentApplicationsByCompanyAsync(int companyId, int count);
        Task<IEnumerable<JobApplication>> GetApplicationsForRecruiterAsync(int recruiterId);
    }
}