using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IEvaluationService
    {
        Task<Evaluation> CreateOrUpdateAsync(int jobApplicationId, decimal? resumeScore, decimal? aiScore, int actingUserId, bool isAdmin);
        Task<Evaluation?> GetByApplicationIdAsync(int jobApplicationId, int actingUserId, bool isAdmin);
    }
}