using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class CandidateRepository : GenericRepository<CandidateProfile>, ICandidateRepository
    {
        public CandidateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CandidateProfile?> GetProfileAsync(int userId)
        {
            return await _context.CandidateProfiles
                .Include(x => x.Skills)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .Include(x => x.Resumes)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<CandidateProfile?> GetCandidateByUserIdAsync(int userId)
        {
            return await _context.CandidateProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<CandidateProfile?> GetCandidateWithAllDetailsAsync(int candidateId)
        {
            return await _context.CandidateProfiles
                .Include(x => x.Skills)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .Include(x => x.Resumes)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == candidateId);
        }

        public async Task<CandidateProfile?> GetCandidateWithResumesAsync(int candidateId)
        {
            return await _context.CandidateProfiles
                .Include(x => x.Resumes)
                .FirstOrDefaultAsync(x => x.Id == candidateId);
        }

        public async Task<IEnumerable<CandidateProfile>> SearchCandidatesAsync(string searchTerm)
        {
            return await _context.CandidateProfiles
                .Where(x =>
                    (x.FirstName != null && x.FirstName.Contains(searchTerm)) ||
                    (x.LastName != null && x.LastName.Contains(searchTerm)) ||
                    (x.Headline != null && x.Headline.Contains(searchTerm)) ||
                    (x.Summary != null && x.Summary.Contains(searchTerm)) ||
                    (x.Location != null && x.Location.Contains(searchTerm)))
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .ToListAsync();
        }

        public async Task<IEnumerable<CandidateProfile>> GetCandidatesBySkillAsync(string skill)
        {
            return await _context.CandidateProfiles
                .Include(x => x.Skills)
                .Where(x => x.Skills.Any(s => s.SkillName.Contains(skill)))
                .Include(x => x.Experiences)
                .ToListAsync();
        }

        public async Task<bool> CandidateExistsAsync(int userId)
        {
            return await _context.CandidateProfiles
                .AnyAsync(x => x.UserId == userId);
        }

        public async Task<int> GetTotalCandidatesCountAsync()
        {
            return await _context.CandidateProfiles.CountAsync();
        }
    }
}