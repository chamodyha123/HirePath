using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface ICandidateService
    {
        Task<CandidateProfile?> GetProfileAsync(int userId);

        Task<IEnumerable<CandidateProfile>> GetAllAsync();

        Task<CandidateProfile> CreateAsync(CandidateProfile profile);

        Task<bool> UpdateAsync(CandidateProfile profile);

        Task<bool> DeleteAsync(int id);
    }
}