using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IApplicationService
    {
        // CandidateProfileId is derived from actingUserId — never trusted from the client.
        Task<JobApplication> ApplyAsync(int jobId, string? coverLetter, int? resumeId, int actingUserId);

        Task<JobApplication?> GetByIdAsync(int id, int actingUserId, bool isAdmin);

        // Always the caller's own applications — never an arbitrary candidateId.
        Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(int actingUserId);

        // Platform Admin oversight only — all applications submitted by a given
        // candidate, across every company. Not exposed to Recruiter/HiringManager.
        Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateProfileId, int actingUserId, bool isAdmin);

        Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId, int actingUserId, bool isAdmin);

        Task<IEnumerable<JobApplication>> GetByCompanyAsync(int actingUserId, bool isAdmin);

        Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? notes, int actingUserId, bool isAdmin);

        // Dedicated workflow-step endpoints — each locks the target status,
        // so a Recruiter calling "shortlist" can never accidentally set "Hired".
        Task<bool> ShortlistAsync(int id, string? notes, int actingUserId, bool isAdmin);
        Task<bool> RejectAsync(int id, string? notes, int actingUserId, bool isAdmin);
        Task<bool> MarkInterviewCompletedAsync(int id, string? notes, int actingUserId, bool isAdmin);
        Task<bool> SendOfferAsync(int id, string? notes, int actingUserId, bool isAdmin);
        Task<bool> MarkHiredAsync(int id, string? notes, int actingUserId, bool isAdmin);

        Task<bool> AddRecruiterNotesAsync(int id, string notes, int actingUserId, bool isAdmin);
        Task<bool> WithdrawAsync(int id, int actingUserId);
        Task<bool> DeleteAsync(int id);
    }
}