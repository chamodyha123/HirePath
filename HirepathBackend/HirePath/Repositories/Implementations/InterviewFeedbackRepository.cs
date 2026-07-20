using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class InterviewFeedbackRepository : IInterviewFeedbackRepository
    {
        private readonly ApplicationDbContext _context;
        public InterviewFeedbackRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(InterviewFeedback feedback) => await _context.InterviewFeedbacks.AddAsync(feedback);

        public Task<InterviewFeedback?> GetByIdAsync(int id) =>
            _context.InterviewFeedbacks.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

        public Task<InterviewFeedback?> GetByInterviewAndUserAsync(int interviewId, int submittedByUserId) =>
            _context.InterviewFeedbacks.FirstOrDefaultAsync(f => f.InterviewId == interviewId && f.SubmittedByUserId == submittedByUserId);

        public async Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId) =>
            await _context.InterviewFeedbacks.AsNoTracking().Where(f => f.InterviewId == interviewId).ToListAsync();

        public async Task<IEnumerable<InterviewFeedback>> GetByJobApplicationIdAsync(int jobApplicationId) =>
            await _context.InterviewFeedbacks.AsNoTracking()
                .Include(f => f.Interview)
                .Where(f => f.Interview != null && f.Interview.JobApplicationId == jobApplicationId)
                .ToListAsync();

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
