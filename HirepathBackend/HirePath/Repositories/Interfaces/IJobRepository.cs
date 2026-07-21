using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetActiveJobsWithSkillsAsync();  // ← ADD THIS
        Task<IEnumerable<Job>> SearchJobsAsync(string keyword);
    }
}
