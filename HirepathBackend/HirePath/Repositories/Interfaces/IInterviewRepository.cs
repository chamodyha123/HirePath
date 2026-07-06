using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task AddAsync(Interview interview);
        Task<Interview?> GetByIdAsync(int id);
        Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId);
        void Update(Interview interview);
        void Delete(Interview interview);
        Task SaveChangesAsync();
    }
}
