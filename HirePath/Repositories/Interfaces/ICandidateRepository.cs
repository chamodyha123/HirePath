using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface ICandidateRepository
        : IGenericRepository<CandidateProfile>
    {
        Task<CandidateProfile?> GetProfileAsync(int userId);
    }
}