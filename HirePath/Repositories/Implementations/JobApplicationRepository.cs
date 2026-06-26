using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobApplication application)
        {
            await _context.JobApplications.AddAsync(application);
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateId)
        {
            return await _context.JobApplications
                .Where(x => x.CandidateProfileId == candidateId)
                .ToListAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId)
        {
            return await _context.JobApplications
                .Where(x => x.JobId == jobId)
                .ToListAsync();
        }

        public void Update(JobApplication application)
        {
            _context.JobApplications.Update(application);
        }

        public void Delete(JobApplication application)
        {
            _context.JobApplications.Remove(application);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}