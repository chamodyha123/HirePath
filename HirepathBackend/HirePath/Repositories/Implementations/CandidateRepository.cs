using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class CandidateRepository
        : GenericRepository<CandidateProfile>,
          ICandidateRepository
    {
        public CandidateRepository(ApplicationDbContext context)
            : base(context)
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
    }
}