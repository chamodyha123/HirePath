using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IJobApplicationService
    {
        Task<JobApplication> ApplyAsync(JobApplication application);

        Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateId);

        Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId);

        Task<JobApplication?> GetByIdAsync(int id);

        Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? feedback);

        Task<bool> DeleteAsync(int id);
    }
}