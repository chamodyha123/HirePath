using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IEvaluationRepository : IGenericRepository<Evaluation>
    {
        Task<Evaluation?> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Evaluation>> GetByCompanyIdAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetByEvaluatorIdAsync(int evaluatorId);
        Task<int?> GetCompanyIdByEvaluationIdAsync(int evaluationId);
        Task<IEnumerable<Evaluation>> GetFinalizedEvaluationsAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetPendingEvaluationsAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate);
        Task<int> GetCountByCompanyAsync(int companyId);
        Task<decimal?> GetAverageOverallScoreByCompanyAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetEvaluationsByScoreRangeAsync(int companyId, decimal minScore, decimal maxScore);
        Task<bool> EvaluationExistsAsync(int applicationId);
    }
}