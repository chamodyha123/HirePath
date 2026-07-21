using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class EvaluationRepository : GenericRepository<Evaluation>, IEvaluationRepository
    {
        public EvaluationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Evaluation?> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .FirstOrDefaultAsync(e => e.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<Evaluation>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .Where(e => e.Application.Job.CompanyId == companyId)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByEvaluatorIdAsync(int evaluatorId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Where(e => e.EvaluatorId == evaluatorId)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<int?> GetCompanyIdByEvaluationIdAsync(int evaluationId)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            return evaluation?.Application?.Job?.CompanyId;
        }

        public async Task<IEnumerable<Evaluation>> GetFinalizedEvaluationsAsync(int companyId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .Where(e => e.Application.Job.CompanyId == companyId && e.IsFinalized)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetPendingEvaluationsAsync(int companyId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .Where(e => e.Application.Job.CompanyId == companyId && !e.IsFinalized)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evaluation>> GetByDateRangeAsync(int companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .Where(e => e.Application.Job.CompanyId == companyId &&
                           e.EvaluationDate >= startDate &&
                           e.EvaluationDate <= endDate)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<int> GetCountByCompanyAsync(int companyId)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Where(e => e.Application.Job.CompanyId == companyId)
                .CountAsync();
        }

        public async Task<decimal?> GetAverageOverallScoreByCompanyAsync(int companyId)
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Where(e => e.Application.Job.CompanyId == companyId && e.OverallScore.HasValue && e.IsFinalized)
                .ToListAsync();

            if (!evaluations.Any())
                return null;

            return evaluations.Average(e => e.OverallScore.Value);
        }

        public async Task<IEnumerable<Evaluation>> GetEvaluationsByScoreRangeAsync(int companyId, decimal minScore, decimal maxScore)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .Where(e => e.Application.Job.CompanyId == companyId &&
                           e.OverallScore.HasValue &&
                           e.OverallScore >= minScore &&
                           e.OverallScore <= maxScore &&
                           e.IsFinalized)
                .OrderByDescending(e => e.OverallScore)
                .ToListAsync();
        }

        public async Task<bool> EvaluationExistsAsync(int applicationId)
        {
            return await _context.Evaluations.AnyAsync(e => e.ApplicationId == applicationId);
        }

        public override async Task<Evaluation?> GetByIdAsync(int id)
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public override async Task<IEnumerable<Evaluation>> GetAllAsync()
        {
            return await _context.Evaluations
                .Include(e => e.Application)
                .ThenInclude(a => a.Job)
                .Include(e => e.Application)
                .ThenInclude(a => a.CandidateProfile)
                .Include(e => e.Evaluator)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }
    }
}