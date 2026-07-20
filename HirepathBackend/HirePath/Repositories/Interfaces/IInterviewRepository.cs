using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task AddAsync(Interview interview);
        Task<Interview?> GetByIdAsync(int id);

        // Loads JobApplication -> Job -> Company so services can
        // verify the interview belongs to the caller's company.
        Task<Interview?> GetByIdWithCompanyAsync(int id);

        Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Interview>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<Interview>> GetAllAsync();
        void Update(Interview interview);
        void Delete(Interview interview);
        Task SaveChangesAsync();
    }
}