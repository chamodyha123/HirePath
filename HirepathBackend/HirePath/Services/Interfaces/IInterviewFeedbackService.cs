using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IInterviewFeedbackService
    {
        Task<InterviewFeedback> SubmitAsync(InterviewFeedback feedback, int actingUserId, bool isAdmin);
        Task<IEnumerable<InterviewFeedback>> GetByApplicationIdAsync(int jobApplicationId, int actingUserId, bool isAdmin);
    }
}