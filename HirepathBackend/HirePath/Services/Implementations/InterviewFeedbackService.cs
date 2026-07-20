using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewFeedbackService : IInterviewFeedbackService
    {
        private readonly IInterviewFeedbackRepository _feedbackRepo;
        private readonly IInterviewRepository _interviewRepo;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationService _applicationService;
        private readonly ILogger<InterviewFeedbackService> _logger;

        public InterviewFeedbackService(
            IInterviewFeedbackRepository feedbackRepo,
            IInterviewRepository interviewRepo,
            IUserRepository userRepo,
            IApplicationService applicationService,
            ILogger<InterviewFeedbackService> logger)
        {
            _feedbackRepo = feedbackRepo;
            _interviewRepo = interviewRepo;
            _userRepo = userRepo;
            _applicationService = applicationService;
            _logger = logger;
        }

        private async Task<bool> HasCompanyAccessAsync(
            Job? job,
            int actingUserId,
            bool isAdmin)
        {
            if (isAdmin)
                return true;

            if (job == null)
                return false;

            var user = await _userRepo.GetByIdAsync(actingUserId);

            return user?.CompanyId == job.CompanyId;
        }

        public async Task<InterviewFeedback> SubmitAsync(
            InterviewFeedback feedback,
            int actingUserId,
            bool isAdmin)
        {
            if (feedback.TechnicalScore is < 1 or > 10 ||
                feedback.CommunicationScore is < 1 or > 10 ||
                feedback.ProblemSolvingScore is < 1 or > 10)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(feedback),
                    "All interview scores must be between 1 and 10.");
            }

            var interview = await _interviewRepo
                .GetByIdWithCompanyAsync(feedback.InterviewId);

            if (interview?.JobApplication == null)
            {
                throw new KeyNotFoundException(
                    "Interview not found.");
            }

            if (!await HasCompanyAccessAsync(
                    interview.JobApplication.Job,
                    actingUserId,
                    isAdmin))
            {
                throw new UnauthorizedAccessException(
                    "You do not have access to this company's recruitment data.");
            }

            if (interview.Status == InterviewStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "Feedback cannot be submitted for a cancelled interview.");
            }

            if (interview.ScheduledAt > DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Feedback cannot be submitted before the interview time.");
            }

            var duplicate = await _feedbackRepo
                .GetByInterviewAndUserAsync(
                    feedback.InterviewId,
                    actingUserId);

            if (duplicate != null)
            {
                throw new InvalidOperationException(
                    "You have already submitted feedback for this interview.");
            }

            feedback.SubmittedByUserId = actingUserId;
            feedback.Comments = string.IsNullOrWhiteSpace(feedback.Comments)
                ? null
                : feedback.Comments.Trim();

            await _feedbackRepo.AddAsync(feedback);
            await _feedbackRepo.SaveChangesAsync();

            // Mark the interview itself as completed.
            interview.Status = InterviewStatus.Completed;
            interview.UpdatedAt = DateTime.UtcNow;

            _interviewRepo.Update(interview);
            await _interviewRepo.SaveChangesAsync();

            // Advance the application workflow only when the current status
            // is InterviewScheduled.
            if (interview.JobApplication.Status ==
                ApplicationStatus.InterviewScheduled)
            {
                await _applicationService.MarkInterviewCompletedAsync(
                    interview.JobApplicationId,
                    "Interview feedback submitted",
                    actingUserId,
                    isAdmin);
            }

            _logger.LogInformation(
                "Feedback {FeedbackId} submitted for interview {InterviewId} by user {UserId}",
                feedback.Id,
                feedback.InterviewId,
                actingUserId);

            return feedback;
        }

        public async Task<IEnumerable<InterviewFeedback>>
            GetByApplicationIdAsync(
                int jobApplicationId,
                int actingUserId,
                bool isAdmin)
        {
            var feedbackList = (
                await _feedbackRepo
                    .GetByJobApplicationIdAsync(jobApplicationId))
                .ToList();

            if (feedbackList.Count == 0)
                return feedbackList;

            // All feedback for one application shares the same job/company,
            // so checking the first interview is sufficient.
            var interview = await _interviewRepo
                .GetByIdWithCompanyAsync(
                    feedbackList[0].InterviewId);

            if (interview?.JobApplication?.Job == null)
            {
                return Enumerable.Empty<InterviewFeedback>();
            }

            var hasAccess = await HasCompanyAccessAsync(
                interview.JobApplication.Job,
                actingUserId,
                isAdmin);

            return hasAccess
                ? feedbackList
                : Enumerable.Empty<InterviewFeedback>();
        }
    }
}