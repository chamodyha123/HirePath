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
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<InterviewFeedback?> GetByInterviewAndUserAsync(
            int interviewId,
            int submittedByUserId)
        {
            return await _context.InterviewFeedbacks
                .FirstOrDefaultAsync(f =>
                    f.InterviewId == interviewId &&
                    f.SubmittedByUserId == submittedByUserId);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId)
        {
            return await _context.InterviewFeedbacks
                .AsNoTracking()
                .Where(f => f.InterviewId == interviewId)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByJobApplicationIdAsync(int jobApplicationId)
        {
            return await _context.InterviewFeedbacks
                .AsNoTracking()
                .Include(f => f.Interview)
                .Where(f =>
                    f.Interview != null &&
                    f.Interview.JobApplicationId == jobApplicationId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}