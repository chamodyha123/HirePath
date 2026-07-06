using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<JobApplication> ApplyAsync(JobApplication application);
        Task<JobApplication?> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateId);
        Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId);
        Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? feedback);
        Task<bool> AddRecruiterNotesAsync(int id, string notes);
        Task<bool> WithdrawAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}