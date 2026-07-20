using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class EvaluationService : IEvaluationService
    {
        // Weights used to combine the three component scores into OverallScore.
        // Adjust here if the team wants different weighting.
        private const decimal ResumeWeight = 0.3m;
        private const decimal AIWeight = 0.3m;
        private const decimal InterviewWeight = 0.4m;

        private readonly IEvaluationRepository _evalRepo;
        private readonly IApplicationRepository _appRepo;
        private readonly IInterviewFeedbackRepository _feedbackRepo;
        private readonly IUserRepository _userRepo;

        public EvaluationService(
            IEvaluationRepository evalRepo,
            IApplicationRepository appRepo,
            IInterviewFeedbackRepository feedbackRepo,
            IUserRepository userRepo)
        {
            _evalRepo = evalRepo;
            _appRepo = appRepo;
            _feedbackRepo = feedbackRepo;
            _userRepo = userRepo;
        }

        private async Task<bool> HasCompanyAccessAsync(Job? job, int actingUserId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            if (job == null)
                return false;

            var user = await _userRepo.GetByIdAsync(actingUserId);
            return user?.CompanyId != null && user.CompanyId == job.CompanyId;
        }

        private async Task<decimal> ComputeInterviewScoreAsync(int jobApplicationId)
        {
            var feedbackList = (await _feedbackRepo.GetByJobApplicationIdAsync(jobApplicationId)).ToList();
            if (feedbackList.Count == 0)
                return 0m;

            var average = feedbackList.Average(f =>
                (f.TechnicalScore + f.CommunicationScore + f.ProblemSolvingScore) / 3.0);

            return Math.Round((decimal)average, 2);
        }

        public async Task<Evaluation> CreateOrUpdateAsync(int jobApplicationId, decimal? resumeScore, decimal? aiScore, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(jobApplicationId);
            if (application == null)
                throw new KeyNotFoundException("Job application not found.");

            if (!await HasCompanyAccessAsync(application.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            var finalResumeScore = resumeScore ?? 0m;
            var finalAIScore = aiScore ?? application.MatchScore ?? 0m;
            var interviewScore = await ComputeInterviewScoreAsync(jobApplicationId);

            var overallScore = Math.Round(
                finalResumeScore * ResumeWeight +
                finalAIScore * AIWeight +
                interviewScore * InterviewWeight,
                2);

            var evaluation = await _evalRepo.GetByJobApplicationIdAsync(jobApplicationId);

            if (evaluation == null)
            {
                evaluation = new Evaluation
                {
                    JobApplicationId = jobApplicationId,
                    ResumeScore = finalResumeScore,
                    AIScore = finalAIScore,
                    InterviewScore = interviewScore,
                    OverallScore = overallScore,
                    EvaluatedByUserId = actingUserId
                };

                await _evalRepo.AddAsync(evaluation);
            }
            else
            {
                evaluation.ResumeScore = finalResumeScore;
                evaluation.AIScore = finalAIScore;
                evaluation.InterviewScore = interviewScore;
                evaluation.OverallScore = overallScore;
                evaluation.EvaluatedByUserId = actingUserId;
                evaluation.UpdatedAt = DateTime.UtcNow;

                _evalRepo.Update(evaluation);
            }

            await _evalRepo.SaveChangesAsync();
            return evaluation;
        }

        public async Task<Evaluation?> GetByApplicationIdAsync(int jobApplicationId, int actingUserId, bool isAdmin)
        {
            var application = await _appRepo.GetByIdWithDetailsAsync(jobApplicationId);
            if (application == null)
                return null;

            if (!await HasCompanyAccessAsync(application.Job, actingUserId, isAdmin))
                return null;

            return await _evalRepo.GetByJobApplicationIdAsync(jobApplicationId);
        }
    }
}