using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<Interview> ScheduleAsync(Interview interview);
        Task<Interview?> GetByIdAsync(int id);
        Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId);
        Task<bool> UpdateAsync(Interview interview);
        Task<bool> EvaluateAsync(int interviewId, decimal score, string? feedback);
        Task<bool> DeleteAsync(int id);
    }
}