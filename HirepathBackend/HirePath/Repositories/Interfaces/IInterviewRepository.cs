using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IInterviewRepository : IGenericRepository<Interview>
    {
        Task<IEnumerable<Interview>> GetInterviewsByApplication(int applicationId);
        Task<IEnumerable<Interview>> GetInterviewsByCompany(int companyId);
        Task<int?> GetCompanyIdByInterviewIdAsync(int interviewId);
    }
}
