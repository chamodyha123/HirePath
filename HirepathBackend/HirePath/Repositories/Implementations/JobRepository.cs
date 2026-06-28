using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class JobRepository
        : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs
                .Where(j => j.IsActive)
                .Include(j => j.Company)
                .Include(j => j.Department)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(string keyword)
        {
            return await _context.Jobs
                .Where(j =>
                    j.Title.Contains(keyword) ||
                    j.Description.Contains(keyword))
                .ToListAsync();
        }
    }
}