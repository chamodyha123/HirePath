using HirePathAI.API.DTOs.JobApplication;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HirePathAI.API.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            IInterviewRepository interviewRepository,
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _feedbackRepository = feedbackRepository;
            _interviewRepository = interviewRepository;
            _applicationRepository = applicationRepository;
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

        public async Task<InterviewFeedback> SubmitFeedbackAsync(SubmitInterviewFeedbackDto dto, int userId)
        {
            // Validate interview exists
            var interview = await _interviewRepository.GetByIdAsync(dto.InterviewId);
            if (interview == null)
                throw new ArgumentException("Interview not found");

            // Validate application exists
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application == null)
                throw new ArgumentException("Application not found");

            // Verify user has access to this company
            var companyId = await _applicationRepository.GetCompanyIdByApplicationIdAsync(dto.ApplicationId);
            if (!companyId.HasValue)
                throw new ArgumentException("Company not found");

            var userCompanyId = await _userRepository.GetUserCompanyIdAsync(userId);
            if (userCompanyId != companyId)
                throw new UnauthorizedAccessException("You don't have access to this application");

            // Check if feedback already exists for this interview
            var existingFeedback = await _feedbackRepository.GetByInterviewIdAsync(dto.InterviewId);
            if (existingFeedback.Any())
                throw new InvalidOperationException("Feedback already submitted for this interview");

            // Calculate overall score
            var scores = new List<decimal?>();
            if (dto.TechnicalScore.HasValue) scores.Add(dto.TechnicalScore);
            if (dto.CommunicationScore.HasValue) scores.Add(dto.CommunicationScore);
            if (dto.ProblemSolvingScore.HasValue) scores.Add(dto.ProblemSolvingScore);
            if (dto.CulturalFitScore.HasValue) scores.Add(dto.CulturalFitScore);

            var overallScore = scores.Any() ? scores.Average() : null;

            var feedback = new InterviewFeedback
            {
                InterviewId = dto.InterviewId,
                ApplicationId = dto.ApplicationId,
                EvaluatorId = userId,
                TechnicalScore = dto.TechnicalScore,
                CommunicationScore = dto.CommunicationScore,
                ProblemSolvingScore = dto.ProblemSolvingScore,
                CulturalFitScore = dto.CulturalFitScore,
                OverallScore = overallScore,
                Comments = dto.Comments,
                Strengths = dto.Strengths,
                Weaknesses = dto.Weaknesses,
                Recommendation = dto.Recommendation,
                FeedbackDate = DateTime.UtcNow,
                IsSubmitted = true
            };

            await _feedbackRepository.AddAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();

            // Update interview status if needed
            if (interview.Status == InterviewStatus.Scheduled || interview.Status == InterviewStatus.Rescheduled)
            {
                interview.Status = InterviewStatus.Completed;
                interview.UpdatedAt = DateTime.UtcNow;
                await _interviewRepository.UpdateAsync(interview);
                await _interviewRepository.SaveChangesAsync();

                // Update application status to Interviewed
                await _applicationRepository.UpdateStatusAsync(
                    dto.ApplicationId,
                    ApplicationStatus.Interviewed,
                    "Interview completed with feedback submitted",
                    userId
                );
            }

            return feedback;
        }

        public async Task<InterviewFeedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByApplicationAsync(int applicationId)
        {
            return await _feedbackRepository.GetByApplicationIdAsync(applicationId);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByInterviewAsync(int interviewId)
        {
            return await _feedbackRepository.GetByInterviewIdAsync(interviewId);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByEvaluatorAsync(int evaluatorId)
        {
            return await _feedbackRepository.GetByEvaluatorIdAsync(evaluatorId);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByCompanyAsync(int companyId)
        {
            return await _feedbackRepository.GetByCompanyIdAsync(companyId);
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, SubmitInterviewFeedbackDto dto, int userId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null)
                return false;

            // Verify access
            if (!await ValidateCompanyAccessAsync(feedbackId, userId))
                throw new UnauthorizedAccessException("You don't have access to this feedback");

            // Only the original evaluator or admin can update
            if (feedback.EvaluatorId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new UnauthorizedAccessException("Only the original evaluator or admin can update feedback");
            }

            // Can only update if not submitted or is admin
            if (feedback.IsSubmitted)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new InvalidOperationException("Cannot update submitted feedback");
            }

            // Update fields
            feedback.TechnicalScore = dto.TechnicalScore ?? feedback.TechnicalScore;
            feedback.CommunicationScore = dto.CommunicationScore ?? feedback.CommunicationScore;
            feedback.ProblemSolvingScore = dto.ProblemSolvingScore ?? feedback.ProblemSolvingScore;
            feedback.CulturalFitScore = dto.CulturalFitScore ?? feedback.CulturalFitScore;
            feedback.Comments = dto.Comments ?? feedback.Comments;
            feedback.Strengths = dto.Strengths ?? feedback.Strengths;
            feedback.Weaknesses = dto.Weaknesses ?? feedback.Weaknesses;
            feedback.Recommendation = dto.Recommendation;
            feedback.UpdatedAt = DateTime.UtcNow;

            // Recalculate overall score
            var scores = new List<decimal?>();
            if (feedback.TechnicalScore.HasValue) scores.Add(feedback.TechnicalScore);
            if (feedback.CommunicationScore.HasValue) scores.Add(feedback.CommunicationScore);
            if (feedback.ProblemSolvingScore.HasValue) scores.Add(feedback.ProblemSolvingScore);
            if (feedback.CulturalFitScore.HasValue) scores.Add(feedback.CulturalFitScore);
            feedback.OverallScore = scores.Any() ? scores.Average() : null;

            await _feedbackRepository.UpdateAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId, int userId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null)
                return false;

            // Only admin can delete
            var user = await _userRepository.GetByIdAsync(userId);
            var roles = await _userRepository.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                throw new UnauthorizedAccessException("Only admin can delete feedback");

            await _feedbackRepository.DeleteAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SubmitFeedbackAsync(int feedbackId, int userId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null)
                return false;

            // Verify access
            if (!await ValidateCompanyAccessAsync(feedbackId, userId))
                throw new UnauthorizedAccessException("You don't have access to this feedback");

            // Only the original evaluator can submit
            if (feedback.EvaluatorId != userId)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var roles = await _userRepository.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    throw new UnauthorizedAccessException("Only the original evaluator can submit feedback");
            }

            feedback.IsSubmitted = true;
            feedback.FeedbackDate = DateTime.UtcNow;
            feedback.UpdatedAt = DateTime.UtcNow;

            await _feedbackRepository.UpdateAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateCompanyAccessAsync(int feedbackId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var userRoles = await _userRepository.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
                return true;

            var companyId = await _feedbackRepository.GetCompanyIdByFeedbackIdAsync(feedbackId);
            if (!companyId.HasValue)
                return false;

            var userCompanyId = await _userRepository.GetUserCompanyIdAsync(userId);
            return userCompanyId == companyId;
        }

        public async Task<bool> CanUserModifyFeedbackAsync(int feedbackId, int userId)
        {
            return await ValidateCompanyAccessAsync(feedbackId, userId);
        }

        public async Task<bool> HasFeedbackBeenSubmittedAsync(int interviewId)
        {
            var feedbacks = await _feedbackRepository.GetByInterviewIdAsync(interviewId);
            return feedbacks.Any(f => f.IsSubmitted);
        }

        public async Task<decimal?> GetAverageScoreByInterviewerAsync(int interviewerId)
        {
            var feedbacks = await _feedbackRepository.GetByEvaluatorIdAsync(interviewerId);
            if (!feedbacks.Any())
                return null;

            return feedbacks.Average(f => f.OverallScore);
        }

        public async Task<Dictionary<HiringRecommendation, int>> GetRecommendationStatsByCompanyAsync(int companyId)
        {
            var feedbacks = await _feedbackRepository.GetByCompanyIdAsync(companyId);
            return feedbacks
                .GroupBy(f => f.Recommendation)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int?> GetUserCompanyIdAsync(int userId)
        {
            return await _userRepository.GetUserCompanyIdAsync(userId);
        }
    }
}
