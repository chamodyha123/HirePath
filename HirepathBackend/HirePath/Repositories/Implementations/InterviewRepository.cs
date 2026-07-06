using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly ApplicationDbContext _context;

        public InterviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            return await _context.Interviews
                .Include(i => i.JobApplication)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Interviews
                .Where(i => i.JobApplicationId == applicationId)
                .ToListAsync();
        }

        public void Update(Interview interview)
        {
            _context.Interviews.Update(interview);
        }

        public void Delete(Interview interview)
        {
            _context.Interviews.Remove(interview);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}