using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IInterviewFeedbackRepository
    {
        Task AddAsync(InterviewFeedback feedback);
        Task<InterviewFeedback?> GetByIdAsync(int id);
        Task<InterviewFeedback?> GetByInterviewAndUserAsync(int interviewId, int submittedByUserId);
        Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId);
        Task<IEnumerable<InterviewFeedback>> GetByJobApplicationIdAsync(int jobApplicationId);
        Task SaveChangesAsync();
    }
}
