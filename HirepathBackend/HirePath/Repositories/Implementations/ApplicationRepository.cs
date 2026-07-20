using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class ApplicationRepository
        : GenericRepository<JobApplication>,
          IApplicationRepository
    {
        public ApplicationRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId)
        {
            return await _context.JobApplications
                .Where(x => x.CandidateProfileId == candidateId)
                .Include(x => x.Job)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetJobApplications(int jobId)
        {
            return await _context.JobApplications
                .Where(x => x.JobId == jobId)
                .Include(x => x.CandidateProfile)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                    .ThenInclude(j => j!.Company)
                .Include(x => x.CandidateProfile)
                .Include(x => x.Interviews)
                .Include(x => x.Evaluation)
                .Include(x => x.StatusHistory)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByCompanyAsync(int companyId)
        {
            return await _context.JobApplications
                .Include(x => x.Job)
                .Include(x => x.CandidateProfile)
                .Where(x => x.Job!.CompanyId == companyId)
                .ToListAsync();
        }
    }
}