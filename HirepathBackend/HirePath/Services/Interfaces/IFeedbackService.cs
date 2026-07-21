using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IFeedbackService
    {
        // ============ FEEDBACK CRUD ============
        Task<InterviewFeedback> SubmitFeedbackAsync(SubmitInterviewFeedbackDto dto, int userId);
        Task<InterviewFeedback?> GetFeedbackByIdAsync(int id);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByApplicationAsync(int applicationId);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByInterviewAsync(int interviewId);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByEvaluatorAsync(int evaluatorId);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByCompanyAsync(int companyId);

        // ============ FEEDBACK MANAGEMENT ============
        Task<bool> UpdateFeedbackAsync(int feedbackId, SubmitInterviewFeedbackDto dto, int userId);
        Task<bool> DeleteFeedbackAsync(int feedbackId, int userId);
        Task<bool> SubmitFeedbackAsync(int feedbackId, int userId);

        // ============ VALIDATION ============
        Task<bool> ValidateCompanyAccessAsync(int feedbackId, int userId);
        Task<bool> CanUserModifyFeedbackAsync(int feedbackId, int userId);
        Task<bool> HasFeedbackBeenSubmittedAsync(int interviewId);

        // ============ STATISTICS ============
        Task<decimal?> GetAverageScoreByInterviewerAsync(int interviewerId);
        Task<Dictionary<HiringRecommendation, int>> GetRecommendationStatsByCompanyAsync(int companyId);

        // ============ USER INFO ============
        // Returns the company id associated with the user (or null if none)
        Task<int?> GetUserCompanyIdAsync(int userId);
    }
}
