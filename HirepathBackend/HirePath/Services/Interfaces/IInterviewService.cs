using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<Interview> ScheduleAsync(Interview interview, int actingUserId, bool isAdmin);
        Task<Interview?> GetByIdAsync(int id, int actingUserId, bool isAdmin);
        Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId, int actingUserId, bool isAdmin);
        Task<IEnumerable<Interview>> GetByCompanyAsync(int actingUserId, bool isAdmin);

        // Field-by-field patch: any parameter left null leaves the existing
        // value on the interview untouched (it is NOT reset to a default).
        Task<bool> UpdateAsync(
            int interviewId,
            DateTime? scheduledAt,
            string? meetingLink,
            string? location,
            string? panel,
            string? notes,
            InterviewStatus? status,
            int actingUserId,
            bool isAdmin);

        Task<bool> CancelAsync(int id, string? notes, int actingUserId, bool isAdmin);
        Task<bool> DeleteAsync(int id);
    }
}