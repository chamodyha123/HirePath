using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IInterviewService
    {
        // ============ INTERVIEW CRUD ============
        Task<Interview> ScheduleInterviewAsync(ScheduleInterviewDto dto, int userId);
        Task<Interview?> GetInterviewByIdAsync(int id);
        Task<IEnumerable<Interview>> GetInterviewsByApplicationAsync(int applicationId);
        Task<IEnumerable<Interview>> GetInterviewsByCompanyAsync(int companyId);
        Task<bool> UpdateInterviewAsync(UpdateInterviewDto dto, int userId);
        Task<bool> CancelInterviewAsync(int interviewId, string? reason, int userId);
        Task<bool> RescheduleInterviewAsync(int interviewId, DateTime newDateTime, int userId);

        // ============ INTERVIEW STATUS ============
        Task<bool> MarkInterviewCompletedAsync(int interviewId, int userId);
        Task<bool> MarkInterviewNoShowAsync(int interviewId, int userId);

        // ============ VALIDATION ============
        Task<bool> ValidateCompanyAccessAsync(int interviewId, int userId);
        Task<bool> CanUserModifyInterviewAsync(int interviewId, int userId);
    }
}