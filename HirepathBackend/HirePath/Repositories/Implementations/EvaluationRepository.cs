using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public EvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Evaluation evaluation)
        {
            await _context.Evaluations.AddAsync(evaluation);
        }

        public async Task<Evaluation?> GetByJobApplicationIdAsync(int jobApplicationId)
        {
            return await _context.Evaluations
                .FirstOrDefaultAsync(e => e.JobApplicationId == jobApplicationId);
        }

        public void Update(Evaluation evaluation)
        {
            _context.Evaluations.Update(evaluation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}