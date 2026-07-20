using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IEvaluationRepository
    {
        Task AddAsync(Evaluation evaluation);
        Task<Evaluation?> GetByJobApplicationIdAsync(int jobApplicationId);
        void Update(Evaluation evaluation);
        Task SaveChangesAsync();
    }
}