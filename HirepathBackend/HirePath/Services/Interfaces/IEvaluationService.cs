using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IEvaluationService
    {
        // ============ EVALUATION CRUD ============
        Task<Evaluation> CreateEvaluationAsync(CreateEvaluationDto dto, int userId);
        Task<Evaluation?> GetEvaluationByIdAsync(int id);
        Task<Evaluation?> GetEvaluationByApplicationAsync(int applicationId);
        Task<IEnumerable<Evaluation>> GetEvaluationsByCompanyAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetEvaluationsByEvaluatorAsync(int evaluatorId);
        Task<bool> UpdateEvaluationAsync(int evaluationId, CreateEvaluationDto dto, int userId);
        Task<bool> DeleteEvaluationAsync(int evaluationId, int userId);
        Task<bool> FinalizeEvaluationAsync(int evaluationId, int userId);

        // ============ EVALUATION SUMMARY ============
        Task<EvaluationSummaryDto> GetEvaluationSummaryAsync(int applicationId);
        Task<decimal?> CalculateOverallScoreAsync(int applicationId);

        // ============ VALIDATION ============
        Task<bool> ValidateCompanyAccessAsync(int evaluationId, int userId);
        Task<bool> CanUserModifyEvaluationAsync(int evaluationId, int userId);
        Task<bool> HasEvaluationBeenFinalizedAsync(int applicationId);

        // ============ STATISTICS ============
        Task<Dictionary<string, decimal>> GetAverageScoresByCompanyAsync(int companyId);
        Task<IEnumerable<Evaluation>> GetEvaluationsByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate);
        Task<int> GetEvaluationCountByCompanyAsync(int companyId);
        // Add this method to the IEvaluationService interface
        Task<int?> GetUserCompanyIdAsync(int userId);

    }

    public class EvaluationSummaryDto
    {
        public int ApplicationId { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }
        public decimal? ResumeScore { get; set; }
        public decimal? AIScore { get; set; }
        public decimal? InterviewScore { get; set; }
        public decimal? HiringManagerScore { get; set; }
        public decimal? OverallScore { get; set; }
        public string? Comments { get; set; }
        public string? Recommendations { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime EvaluationDate { get; set; }
        public string? EvaluatorName { get; set; }

        // Score breakdown for display
        public Dictionary<string, decimal> ScoreBreakdown { get; set; } = new();
        public string? ScoreLabel { get; set; }
        public string? ScoreColor { get; set; }
    }
}