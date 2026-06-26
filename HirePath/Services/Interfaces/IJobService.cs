using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IJobService
    {
        Task<IEnumerable<Job>> GetAllAsync();

        Task<IEnumerable<Job>> GetActiveJobsAsync();

        Task<IEnumerable<Job>> SearchJobsAsync(string keyword);

        Task<Job?> GetByIdAsync(int id);

        Task<Job> CreateAsync(Job job);

        Task<bool> UpdateAsync(Job job);

        Task<bool> DeleteAsync(int id);
    }
}