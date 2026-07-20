using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IApplicationStatusHistoryRepository
    {
        Task AddAsync(ApplicationStatusHistory history);
        Task<IEnumerable<ApplicationStatusHistory>> GetByJobApplicationIdAsync(int jobApplicationId);
        Task SaveChangesAsync();
    }
}