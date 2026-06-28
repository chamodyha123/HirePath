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
    }
}