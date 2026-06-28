using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<IEnumerable<Job>> GetActiveJobsAsync();

        Task<IEnumerable<Job>> SearchJobsAsync(string keyword);
    }
}