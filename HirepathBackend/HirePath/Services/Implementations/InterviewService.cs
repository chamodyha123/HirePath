using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repo;

        public InterviewService(IInterviewRepository repo)
        {
            _repo = repo;
        }

        public async Task<Interview> ScheduleAsync(Interview interview)
        {
            interview.Status = InterviewStatus.Scheduled;
            await _repo.AddAsync(interview);
            await _repo.SaveChangesAsync();
            return interview;
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId)
        {
            return await _repo.GetByApplicationIdAsync(applicationId);
        }

        public async Task<bool> UpdateAsync(Interview interview)
        {
            var existing = await _repo.GetByIdAsync(interview.Id);

            if (existing == null)
                return false;

            existing.ScheduledAt = interview.ScheduledAt;
            existing.MeetingLink = interview.MeetingLink;
            existing.Status = interview.Status;

            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EvaluateAsync(int interviewId, decimal score, string? feedback)
        {
            var interview = await _repo.GetByIdAsync(interviewId);

            if (interview == null)
                return false;

            interview.Score = score;
            interview.Feedback = feedback;
            interview.Status = InterviewStatus.Completed;

            _repo.Update(interview);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interview = await _repo.GetByIdAsync(id);

            if (interview == null)
                return false;

            _repo.Delete(interview);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}