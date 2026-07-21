using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class FeedbackRepository : GenericRepository<InterviewFeedback>, IFeedbackRepository
    {
        public FeedbackRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.InterviewFeedbacks
                .Where(f => f.ApplicationId == applicationId)
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByInterviewIdAsync(int interviewId)
        {
            return await _context.InterviewFeedbacks
                .Where(f => f.InterviewId == interviewId)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<InterviewFeedback?> GetByInterviewIdSingleAsync(int interviewId)
        {
            return await _context.InterviewFeedbacks
                .FirstOrDefaultAsync(f => f.InterviewId == interviewId);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByEvaluatorIdAsync(int evaluatorId)
        {
            return await _context.InterviewFeedbacks
                .Where(f => f.EvaluatorId == evaluatorId)
                .Include(f => f.Interview)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterviewFeedback>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .ThenInclude(j => j.Company)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Where(f => f.Application.Job.CompanyId == companyId)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<int?> GetCompanyIdByFeedbackIdAsync(int feedbackId)
        {
            var feedback = await _context.InterviewFeedbacks
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(f => f.Id == feedbackId);

            return feedback?.Application?.Job?.CompanyId;
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByRecommendationAsync(int companyId, HiringRecommendation recommendation)
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Where(f => f.Application.Job.CompanyId == companyId && f.Recommendation == recommendation)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<decimal?> GetAverageOverallScoreByCompanyAsync(int companyId)
        {
            var feedbacks = await _context.InterviewFeedbacks
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Where(f => f.Application.Job.CompanyId == companyId && f.OverallScore.HasValue && f.IsSubmitted)
                .ToListAsync();

            if (!feedbacks.Any())
                return null;

            return feedbacks.Average(f => f.OverallScore.Value);
        }

        public async Task<Dictionary<HiringRecommendation, int>> GetRecommendationDistributionAsync(int companyId)
        {
            var feedbacks = await _context.InterviewFeedbacks
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Where(f => f.Application.Job.CompanyId == companyId && f.IsSubmitted)
                .GroupBy(f => f.Recommendation)
                .Select(g => new { Recommendation = g.Key, Count = g.Count() })
                .ToListAsync();

            return feedbacks.ToDictionary(x => x.Recommendation, x => x.Count);
        }

        public async Task<IEnumerable<InterviewFeedback>> GetFeedbackByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Where(f => f.Application.Job.CompanyId == companyId &&
                           f.FeedbackDate >= startDate &&
                           f.FeedbackDate <= endDate)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }

        public async Task<bool> FeedbackExistsAsync(int interviewId)
        {
            return await _context.InterviewFeedbacks.AnyAsync(f => f.InterviewId == interviewId);
        }

        public new async Task<InterviewFeedback?> GetByIdAsync(int id)
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public new async Task<IEnumerable<InterviewFeedback>> GetAllAsync()
        {
            return await _context.InterviewFeedbacks
                .Include(f => f.Interview)
                .Include(f => f.Evaluator)
                .Include(f => f.Application)
                .ThenInclude(a => a.Job)
                .Include(f => f.Application)
                .ThenInclude(a => a.CandidateProfile)
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();
        }
    }
}
