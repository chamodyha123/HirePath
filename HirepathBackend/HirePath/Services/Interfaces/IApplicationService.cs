using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IApplicationService
    {
        // ============ APPLICATION CRUD ============
        Task<JobApplication> ApplyAsync(CreateApplicationDto dto, int userId);
        Task<JobApplication?> GetApplicationByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetApplicationsByCandidateAsync(int candidateProfileId);
        Task<IEnumerable<JobApplication>> GetApplicationsByJobAsync(int jobId);
        Task<IEnumerable<JobApplication>> GetApplicationsByCompanyAsync(int companyId);
        Task<bool> UpdateStatusAsync(int applicationId, ApplicationStatus status, string? notes, int userId);
        Task<bool> DeleteApplicationAsync(int id);

        // ============ WORKFLOW ACTIONS ============
        Task<bool> ShortlistAsync(int applicationId, string? notes, int userId);
        Task<bool> RejectAsync(int applicationId, string? notes, int userId);
        Task<bool> ScheduleInterviewAsync(int applicationId, WorkflowActionDto dto, int userId);
        Task<bool> SendOfferAsync(int applicationId, WorkflowActionDto dto, int userId);
        Task<bool> HireAsync(int applicationId, string? notes, int userId);
        Task<bool> WithdrawApplicationAsync(int applicationId, int userId);

        // ============ STATUS TRANSITIONS ============
        bool CanTransitionTo(ApplicationStatus currentStatus, ApplicationStatus newStatus, string userRole);
        Task<IEnumerable<ApplicationStatusHistory>> GetStatusHistoryAsync(int applicationId);
        Task<bool> ValidateCompanyAccessAsync(int applicationId, int userId);
    }
}