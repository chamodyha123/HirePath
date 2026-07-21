using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IFeedbackRepository : IGenericRepository<InterviewFeedback>
    {
        Task<IEnumerable<InterviewFeedback>> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId);
        Task<IEnumerable<InterviewFeedback>> GetByEvaluatorIdAsync(int evaluatorId);
        Task<IEnumerable<InterviewFeedback>> GetByCompanyIdAsync(int companyId);
        Task<InterviewFeedback?> GetByInterviewIdSingleAsync(int interviewId);
        Task<int?> GetCompanyIdByFeedbackIdAsync(int feedbackId);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByRecommendationAsync(int companyId, HiringRecommendation recommendation);
        Task<decimal?> GetAverageOverallScoreByCompanyAsync(int companyId);
        Task<Dictionary<HiringRecommendation, int>> GetRecommendationDistributionAsync(int companyId);
        Task<IEnumerable<InterviewFeedback>> GetFeedbackByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate);
        Task<bool> FeedbackExistsAsync(int interviewId);
    }
}