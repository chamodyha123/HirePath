using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class InterviewFeedbackRepository : IInterviewFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public InterviewFeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InterviewFeedback feedback)
        {
            await _context.InterviewFeedbacks.AddAsync(feedback);
        }

        public async Task<InterviewFeedback?> GetByIdAsync(int id)
        {
            return await _context.InterviewFeedbacks
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId)
        {
            return await _context.InterviewFeedbacks
                .Where(f => f.InterviewId == interviewId)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByJobApplicationIdAsync(int jobApplicationId)
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Where(f => f.Interview!.JobApplicationId == jobApplicationId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}