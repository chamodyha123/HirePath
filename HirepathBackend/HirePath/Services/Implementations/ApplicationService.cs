using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IJobApplicationRepository _repo;

        public ApplicationService(IJobApplicationRepository repo)
        {
            _repo = repo;
        }

        public async Task<JobApplication> ApplyAsync(JobApplication application)
        {
            application.AppliedDate = DateTime.UtcNow;
            application.Status = ApplicationStatus.Applied;

            await _repo.AddAsync(application);
            await _repo.SaveChangesAsync();

            return application;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<JobApplication>> GetByCandidateAsync(int candidateId)
        {
            return await _repo.GetByCandidateAsync(candidateId);
        }

        public async Task<IEnumerable<JobApplication>> GetByJobAsync(int jobId)
        {
            return await _repo.GetByJobAsync(jobId);
        }

        public async Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? feedback)
        {
            var app = await _repo.GetByIdAsync(id);

            if (app == null)
                return false;

            app.Status = status;
            app.RecruiterNotes = feedback;
            app.UpdatedAt = DateTime.UtcNow;

            _repo.Update(app);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddRecruiterNotesAsync(int id, string notes)
        {
            var app = await _repo.GetByIdAsync(id);

            if (app == null)
                return false;

            app.RecruiterNotes = notes;
            app.UpdatedAt = DateTime.UtcNow;

            _repo.Update(app);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> WithdrawAsync(int id)
        {
            var app = await _repo.GetByIdAsync(id);

            if (app == null)
                return false;

            app.Status = ApplicationStatus.Withdrawn;
            app.UpdatedAt = DateTime.UtcNow;

            _repo.Update(app);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var app = await _repo.GetByIdAsync(id);

            if (app == null)
                return false;

            _repo.Delete(app);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}