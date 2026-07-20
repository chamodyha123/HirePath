using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IApplicationService
    {
        // CandidateProfileId is derived from actingUserId
        // and is never trusted from the client.
        Task<JobApplication> ApplyAsync(
            int jobId,
            string? coverLetter,
            int? resumeId,
            int actingUserId);

        Task<JobApplication?> GetByIdAsync(
            int id,
            int actingUserId,
            bool isAdmin);

        // Always returns the caller's own applications.
        Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(
            int actingUserId);

        // Platform Admin oversight only:
        // all applications submitted by a given candidate.
        Task<IEnumerable<JobApplication>> GetByCandidateAsync(
            int candidateProfileId,
            int actingUserId,
            bool isAdmin);

        Task<IEnumerable<JobApplication>> GetByJobAsync(
            int jobId,
            int actingUserId,
            bool isAdmin);

        Task<IEnumerable<JobApplication>> GetByCompanyAsync(
            int actingUserId,
            bool isAdmin);

        Task<IEnumerable<ApplicationStatusHistory>> GetHistoryAsync(
            int applicationId,
            int actingUserId,
            bool isAdmin);

        Task<bool> UpdateStatusAsync(
            int id,
            ApplicationStatus status,
            string? notes,
            int actingUserId,
            bool isAdmin);

        // Dedicated workflow methods prevent callers from setting
        // an unintended application status.
        Task<bool> ShortlistAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> RejectAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> MarkInterviewCompletedAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> SendOfferAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> MarkHiredAsync(
            int id,
            string? notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> AddRecruiterNotesAsync(
            int id,
            string notes,
            int actingUserId,
            bool isAdmin);

        Task<bool> WithdrawAsync(
            int id,
            int actingUserId);

        Task<bool> DeleteAsync(int id);
    }
}