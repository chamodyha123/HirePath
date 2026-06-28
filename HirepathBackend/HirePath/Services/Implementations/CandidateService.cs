using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<IEnumerable<CandidateProfile>> GetAllAsync()
        {
            return await _candidateRepository.GetAllAsync();
        }

        public async Task<CandidateProfile?> GetProfileAsync(int userId)
        {
            return await _candidateRepository.GetProfileAsync(userId);
        }

        public async Task<CandidateProfile> CreateAsync(CandidateProfile profile)
        {
            await _candidateRepository.AddAsync(profile);
            await _candidateRepository.SaveChangesAsync();

            return profile;
        }

        public async Task<bool> UpdateAsync(CandidateProfile profile)
        {
            var existing = await _candidateRepository.GetByIdAsync(profile.Id);

            if (existing == null)
                return false;

            _candidateRepository.Update(profile);
            await _candidateRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var profile = await _candidateRepository.GetByIdAsync(id);

            if (profile == null)
                return false;

            _candidateRepository.Delete(profile);
            await _candidateRepository.SaveChangesAsync();

            return true;
        }
    }
}