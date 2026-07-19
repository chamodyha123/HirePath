using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface ICandidateRepository : IGenericRepository<CandidateProfile>
    {
        Task<CandidateProfile?> GetProfileAsync(int userId);
        Task<CandidateProfile?> GetCandidateByUserIdAsync(int userId);
        Task<CandidateProfile?> GetCandidateWithAllDetailsAsync(int candidateId);
        Task<CandidateProfile?> GetCandidateWithResumesAsync(int candidateId);
        Task<IEnumerable<CandidateProfile>> SearchCandidatesAsync(string searchTerm);
        Task<IEnumerable<CandidateProfile>> GetCandidatesBySkillAsync(string skill);
        Task<bool> CandidateExistsAsync(int userId);
        Task<int> GetTotalCandidatesCountAsync();
    }
}