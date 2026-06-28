using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task AddAsync(JobApplication application);
        Task<JobApplication?> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateId);
        Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId);

        void Update(JobApplication application);
        void Delete(JobApplication application);
        Task SaveChangesAsync();
    }
}