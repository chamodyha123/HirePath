using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HirePathAI.API.Services.Implementations
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EvaluationService(
            IEvaluationRepository evaluationRepository,
            IApplicationRepository applicationRepository,
            IInterviewRepository interviewRepository,
            IFeedbackRepository feedbackRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _evaluationRepository = evaluationRepository;
            _applicationRepository = applicationRepository;
            _interviewRepository = interviewRepository;
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");
            return int.Parse(userIdClaim);
        }

        public async Task<Evaluation> CreateEvaluationAsync(CreateEvaluationDto dto, int userId)
        {
            // Validate application exists
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application == null)
                throw new ArgumentException("Application not found");

            // Verify user has access to this company
            var companyId = await _applicationRepository.GetCompanyIdByApplicationIdAsync(dto.ApplicationId);
            if (!companyId.HasValue)
                throw new ArgumentException("Company not found");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var userCompanyId = user.CompanyId;
            if (userCompanyId != companyId)
                throw new UnauthorizedAccessException("You don't have access to this application");

            // Check if evaluation already exists
            var existingEvaluation = await _evaluationRepository.GetByApplicationIdAsync(dto.ApplicationId);
            if (existingEvaluation != null)
                throw new InvalidOperationException("Evaluation already exists for this application");

            // Calculate overall score
            var overallScore = CalculateOverallScore(
                dto.ResumeScore,
                dto.AIScore,
                dto.InterviewScore,
                dto.HiringManagerScore
            );

            var evaluation = new Evaluation
            {
                ApplicationId = dto.ApplicationId,
                EvaluatorId = userId,
                ResumeScore = dto.ResumeScore,
                AIScore = dto.AIScore,
                InterviewScore = dto.InterviewScore,
                HiringManagerScore = dto.HiringManagerScore,
                OverallScore = overallScore,
                Comments = dto.Comments,
                Recommendations = dto.Recommendations,
                IsFinalized = dto.IsFinalized,
                EvaluationDate = DateTime.UtcNow
            };

            await _evaluationRepository.AddAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();

            // If finalized, update application status if appropriate
            if (dto.IsFinalized && overallScore.HasValue)
            {
                if (overallScore >= 70)
                {
                    await _applicationRepository.UpdateStatusAsync(
                        dto.ApplicationId,
                        ApplicationStatus.Offered,
                        $"Evaluation finalized with score {overallScore:F1}% - Candidate recommended for offer",
                        userId
                    );
                }
                else if (overallScore >= 50)
                {
                    // Keep as interviewed, maybe consider for other positions
                    await _applicationRepository.UpdateStatusAsync(
                        dto.ApplicationId,
                        ApplicationStatus.Interviewed,
                        $"Evaluation finalized with score {overallScore:F1}% - Candidate under consideration",
                        userId
                    );
                }
                else
                {
                    await _applicationRepository.UpdateStatusAsync(
                        dto.ApplicationId,
                        ApplicationStatus.Rejected,
                        $"Evaluation finalized with score {overallScore:F1}% - Candidate not selected",
                        userId
                    );
                }
            }

            return evaluation;
        }

        public async Task<Evaluation?> GetEvaluationByIdAsync(int id)
        {
            return await _evaluationRepository.GetByIdAsync(id);
        }

        public async Task<Evaluation?> GetEvaluationByApplicationAsync(int applicationId)
        {
            return await _evaluationRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<Evaluation>> GetEvaluationsByCompanyAsync(int companyId)
        {
            return await _evaluationRepository.GetByCompanyIdAsync(companyId);
        }

        public async Task<IEnumerable<Evaluation>> GetEvaluationsByEvaluatorAsync(int evaluatorId)
        {
            return await _evaluationRepository.GetByEvaluatorIdAsync(evaluatorId);
        }

        public async Task<bool> UpdateEvaluationAsync(int evaluationId, CreateEvaluationDto dto, int userId)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(evaluationId);
            if (evaluation == null)
                return false;

            // Verify access
            if (!await ValidateCompanyAccessAsync(evaluationId, userId))
                throw new UnauthorizedAccessException("You don't have access to this evaluation");

            // Only the original evaluator or admin can update
            if (evaluation.EvaluatorId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new UnauthorizedAccessException("Only the original evaluator or admin can update evaluation");
            }

            // Can only update if not finalized or is admin
            if (evaluation.IsFinalized)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new InvalidOperationException("Cannot update finalized evaluation");
            }

            // Update fields
            evaluation.ResumeScore = dto.ResumeScore ?? evaluation.ResumeScore;
            evaluation.AIScore = dto.AIScore ?? evaluation.AIScore;
            evaluation.InterviewScore = dto.InterviewScore ?? evaluation.InterviewScore;
            evaluation.HiringManagerScore = dto.HiringManagerScore ?? evaluation.HiringManagerScore;
            evaluation.Comments = dto.Comments ?? evaluation.Comments;
            evaluation.Recommendations = dto.Recommendations ?? evaluation.Recommendations;
            evaluation.IsFinalized = dto.IsFinalized;
            evaluation.UpdatedAt = DateTime.UtcNow;

            // Recalculate overall score
            evaluation.OverallScore = CalculateOverallScore(
                evaluation.ResumeScore,
                evaluation.AIScore,
                evaluation.InterviewScore,
                evaluation.HiringManagerScore
            );

            await _evaluationRepository.UpdateAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEvaluationAsync(int evaluationId, int userId)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(evaluationId);
            if (evaluation == null)
                return false;

            // Only admin can delete
            var user = await _userRepository.GetByIdAsync(userId);
            var roles = await _userRepository.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                throw new UnauthorizedAccessException("Only admin can delete evaluation");

            await _evaluationRepository.DeleteAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FinalizeEvaluationAsync(int evaluationId, int userId)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(evaluationId);
            if (evaluation == null)
                return false;

            // Verify access
            if (!await ValidateCompanyAccessAsync(evaluationId, userId))
                throw new UnauthorizedAccessException("You don't have access to this evaluation");

            // Only the original evaluator or admin can finalize
            if (evaluation.EvaluatorId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new UnauthorizedAccessException("Only the original evaluator or admin can finalize evaluation");
            }

            evaluation.IsFinalized = true;
            evaluation.EvaluationDate = DateTime.UtcNow;
            evaluation.UpdatedAt = DateTime.UtcNow;

            await _evaluationRepository.UpdateAsync(evaluation);
            await _evaluationRepository.SaveChangesAsync();

            // Update application status based on score
            if (evaluation.OverallScore.HasValue)
            {
                if (evaluation.OverallScore >= 70)
                {
                    await _applicationRepository.UpdateStatusAsync(
                        evaluation.ApplicationId,
                        ApplicationStatus.Offered,
                        $"Evaluation finalized with score {evaluation.OverallScore:F1}% - Candidate recommended for offer",
                        userId
                    );
                }
                else if (evaluation.OverallScore >= 50)
                {
                    await _applicationRepository.UpdateStatusAsync(
                        evaluation.ApplicationId,
                        ApplicationStatus.Interviewed,
                        $"Evaluation finalized with score {evaluation.OverallScore:F1}% - Candidate under consideration",
                        userId
                    );
                }
                else
                {
                    await _applicationRepository.UpdateStatusAsync(
                        evaluation.ApplicationId,
                        ApplicationStatus.Rejected,
                        $"Evaluation finalized with score {evaluation.OverallScore:F1}% - Candidate not selected",
                        userId
                    );
                }
            }

            return true;
        }

        public async Task<EvaluationSummaryDto> GetEvaluationSummaryAsync(int applicationId)
        {
            var evaluation = await _evaluationRepository.GetByApplicationIdAsync(applicationId);
            if (evaluation == null)
                return null;

            var application = await _applicationRepository.GetByIdWithDetailsAsync(applicationId);
            var evaluator = await _userRepository.GetByIdAsync(evaluation.EvaluatorId);

            var summary = new EvaluationSummaryDto
            {
                ApplicationId = evaluation.ApplicationId,
                CandidateName = $"{application?.CandidateProfile?.FirstName} {application?.CandidateProfile?.LastName}",
                JobTitle = application?.Job?.Title,
                ResumeScore = evaluation.ResumeScore,
                AIScore = evaluation.AIScore,
                InterviewScore = evaluation.InterviewScore,
                HiringManagerScore = evaluation.HiringManagerScore,
                OverallScore = evaluation.OverallScore,
                Comments = evaluation.Comments,
                Recommendations = evaluation.Recommendations,
                IsFinalized = evaluation.IsFinalized,
                EvaluationDate = evaluation.EvaluationDate,
                EvaluatorName = evaluator?.FullName,
                ScoreBreakdown = new Dictionary<string, decimal>()
            };

            // Add breakdown
            if (evaluation.ResumeScore.HasValue)
                summary.ScoreBreakdown["Resume Score"] = evaluation.ResumeScore.Value;
            if (evaluation.AIScore.HasValue)
                summary.ScoreBreakdown["AI Match"] = evaluation.AIScore.Value;
            if (evaluation.InterviewScore.HasValue)
                summary.ScoreBreakdown["Interview Score"] = evaluation.InterviewScore.Value;
            if (evaluation.HiringManagerScore.HasValue)
                summary.ScoreBreakdown["Hiring Manager Score"] = evaluation.HiringManagerScore.Value;

            // Add label and color based on score
            if (evaluation.OverallScore.HasValue)
            {
                var score = evaluation.OverallScore.Value;
                if (score >= 80)
                {
                    summary.ScoreLabel = "Excellent Candidate";
                    summary.ScoreColor = "Green";
                }
                else if (score >= 70)
                {
                    summary.ScoreLabel = "Strong Candidate";
                    summary.ScoreColor = "Blue";
                }
                else if (score >= 60)
                {
                    summary.ScoreLabel = "Good Candidate";
                    summary.ScoreColor = "Orange";
                }
                else if (score >= 50)
                {
                    summary.ScoreLabel = "Average Candidate";
                    summary.ScoreColor = "Yellow";
                }
                else
                {
                    summary.ScoreLabel = "Needs Improvement";
                    summary.ScoreColor = "Red";
                }
            }

            return summary;
        }

        public async Task<decimal?> CalculateOverallScoreAsync(int applicationId)
        {
            var evaluation = await _evaluationRepository.GetByApplicationIdAsync(applicationId);
            if (evaluation == null)
                return null;

            return CalculateOverallScore(
                evaluation.ResumeScore,
                evaluation.AIScore,
                evaluation.InterviewScore,
                evaluation.HiringManagerScore
            );
        }

        public async Task<bool> ValidateCompanyAccessAsync(int evaluationId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var userRoles = await _userRepository.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
                return true;

            var companyId = await _evaluationRepository.GetCompanyIdByEvaluationIdAsync(evaluationId);
            if (!companyId.HasValue)
                return false;

            var userCompanyId = await _userRepository.GetUserCompanyIdAsync(userId);
            return userCompanyId == companyId;
        }

        public async Task<bool> CanUserModifyEvaluationAsync(int evaluationId, int userId)
        {
            return await ValidateCompanyAccessAsync(evaluationId, userId);
        }

        public async Task<bool> HasEvaluationBeenFinalizedAsync(int applicationId)
        {
            var evaluation = await _evaluationRepository.GetByApplicationIdAsync(applicationId);
            return evaluation != null && evaluation.IsFinalized;
        }

        public async Task<Dictionary<string, decimal>> GetAverageScoresByCompanyAsync(int companyId)
        {
            var evaluations = await _evaluationRepository.GetByCompanyIdAsync(companyId);
            if (!evaluations.Any())
                return new Dictionary<string, decimal>();

            var result = new Dictionary<string, decimal>();

            if (evaluations.Any(e => e.ResumeScore.HasValue))
                result["Average Resume Score"] = evaluations.Where(e => e.ResumeScore.HasValue).Average(e => e.ResumeScore.Value);

            if (evaluations.Any(e => e.AIScore.HasValue))
                result["Average AI Score"] = evaluations.Where(e => e.AIScore.HasValue).Average(e => e.AIScore.Value);

            if (evaluations.Any(e => e.InterviewScore.HasValue))
                result["Average Interview Score"] = evaluations.Where(e => e.InterviewScore.HasValue).Average(e => e.InterviewScore.Value);

            if (evaluations.Any(e => e.HiringManagerScore.HasValue))
                result["Average Hiring Manager Score"] = evaluations.Where(e => e.HiringManagerScore.HasValue).Average(e => e.HiringManagerScore.Value);

            if (evaluations.Any(e => e.OverallScore.HasValue))
                result["Average Overall Score"] = evaluations.Where(e => e.OverallScore.HasValue).Average(e => e.OverallScore.Value);

            return result;
        }

        public async Task<IEnumerable<Evaluation>> GetEvaluationsByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate)
        {
            return await _evaluationRepository.GetByDateRangeAsync(companyId, startDate, endDate);
        }

        public async Task<int> GetEvaluationCountByCompanyAsync(int companyId)
        {
            return await _evaluationRepository.GetCountByCompanyAsync(companyId);
        }

        // ============ PRIVATE HELPERS ============

        private decimal? CalculateOverallScore(decimal? resumeScore, decimal? aiScore, decimal? interviewScore, decimal? hiringManagerScore)
        {
            var scores = new List<decimal?>();
            if (resumeScore.HasValue) scores.Add(resumeScore);
            if (aiScore.HasValue) scores.Add(aiScore);
            if (interviewScore.HasValue) scores.Add(interviewScore);
            if (hiringManagerScore.HasValue) scores.Add(hiringManagerScore);

            if (!scores.Any())
                return null;

            return scores.Average();
        }

        // Extension method for IEvaluationService
        public async Task<int?> GetUserCompanyIdAsync(int userId)
        {
            return await _userRepository.GetUserCompanyIdAsync(userId);
        }
    }
}