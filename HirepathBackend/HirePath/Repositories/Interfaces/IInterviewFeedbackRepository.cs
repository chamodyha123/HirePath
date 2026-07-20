using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IInterviewFeedbackRepository
    {
        Task AddAsync(InterviewFeedback feedback);
        Task<InterviewFeedback?> GetByIdAsync(int id);
        Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId);

        // All feedback for a given JobApplication (joins through Interviews)
        Task<IEnumerable<InterviewFeedback>> GetByJobApplicationIdAsync(int jobApplicationId);
        Task SaveChangesAsync();
    }
}