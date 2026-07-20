using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewFeedbackService : IInterviewFeedbackService
    {
        private readonly IInterviewFeedbackRepository _feedbackRepo;
        private readonly IInterviewRepository _interviewRepo;
        private readonly IUserRepository _userRepo;
        private readonly IApplicationService _applicationService;

        public InterviewFeedbackService(
            IInterviewFeedbackRepository feedbackRepo,
            IInterviewRepository interviewRepo,
            IUserRepository userRepo,
            IApplicationService applicationService)
        {
            _feedbackRepo = feedbackRepo;
            _interviewRepo = interviewRepo;
            _userRepo = userRepo;
            _applicationService = applicationService;
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

        public async Task<InterviewFeedback> SubmitAsync(InterviewFeedback feedback, int actingUserId, bool isAdmin)
        {
            var interview = await _interviewRepo.GetByIdWithCompanyAsync(feedback.InterviewId);
            if (interview?.JobApplication == null)
                throw new KeyNotFoundException("Interview not found.");

            if (!await HasCompanyAccessAsync(interview.JobApplication.Job, actingUserId, isAdmin))
                throw new UnauthorizedAccessException("You do not have access to this company's recruitment data.");

            feedback.SubmittedByUserId = actingUserId;

            await _feedbackRepo.AddAsync(feedback);
            await _feedbackRepo.SaveChangesAsync();

            // Mark the interview itself completed
            interview.Status = InterviewStatus.Completed;
            interview.UpdatedAt = DateTime.UtcNow;
            _interviewRepo.Update(interview);
            await _interviewRepo.SaveChangesAsync();

            // Advance the application workflow to "Interview Completed"
            await _applicationService.MarkInterviewCompletedAsync(
                interview.JobApplicationId,
                "Interview feedback submitted",
                actingUserId,
                isAdmin);

            return feedback;
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByApplicationIdAsync(int jobApplicationId, int actingUserId, bool isAdmin)
        {
            var feedbackList = (await _feedbackRepo.GetByJobApplicationIdAsync(jobApplicationId)).ToList();
            if (feedbackList.Count == 0)
                return feedbackList;

            // All feedback for one application shares the same job -> company,
            // so checking against the first interview's job is sufficient.
            var interview = await _interviewRepo.GetByIdWithCompanyAsync(feedbackList[0].InterviewId);
            if (interview?.JobApplication?.Job == null)
                return Enumerable.Empty<InterviewFeedback>();

            if (!await HasCompanyAccessAsync(interview.JobApplication.Job, actingUserId, isAdmin))
                return Enumerable.Empty<InterviewFeedback>();

            return feedbackList;
        }
    }
}