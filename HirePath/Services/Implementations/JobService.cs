using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _jobRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _jobRepository.GetActiveJobsAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(string keyword)
        {
            return await _jobRepository.SearchJobsAsync(keyword);
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _jobRepository.GetByIdAsync(id);
        }

        public async Task<Job> CreateAsync(Job job)
        {
            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            return job;
        }

        public async Task<bool> UpdateAsync(Job job)
        {
            var existing = await _jobRepository.GetByIdAsync(job.Id);

            if (existing == null)
                return false;

            _jobRepository.Update(job);
            await _jobRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                return false;

            _jobRepository.Delete(job);
            await _jobRepository.SaveChangesAsync();

            return true;
        }
    }
}