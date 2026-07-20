using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class ApplicationStatusHistoryRepository : IApplicationStatusHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationStatusHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationStatusHistory history)
        {
            await _context.ApplicationStatusHistories.AddAsync(history);
        }

        public async Task<IEnumerable<ApplicationStatusHistory>> GetByJobApplicationIdAsync(int jobApplicationId)
        {
            return await _context.ApplicationStatusHistories
                .AsNoTracking()
                .Where(h => h.JobApplicationId == jobApplicationId)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}