// File: HirepathBackend/HirePath/API/Repositories/Implementations/InterviewRepository.cs
using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.Repositories; // if GenericRepository<T> lives here
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class InterviewRepository : GenericRepository<Interview>, IInterviewRepository
    {
        public InterviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<IEnumerable<Interview>> GetInterviewsByApplication(int applicationId)
        {
            return FindAsync(i => i.JobApplicationId == applicationId);
        }

        public Task<IEnumerable<Interview>> GetInterviewsByCompany(int companyId)
        {
            return FindAsync(i => i.JobApplication != null && i.JobApplication.Job != null && i.JobApplication.Job.CompanyId == companyId);
        }

        public async Task<int?> GetCompanyIdByInterviewIdAsync(int interviewId)
        {
            return await _context.Set<Interview>()
                .Where(i => i.Id == interviewId && i.JobApplication != null && i.JobApplication.Job != null)
                .Select(i => (int?)i.JobApplication.Job.CompanyId)
                .FirstOrDefaultAsync();
        }

        // Add other interview-specific data access methods here if needed
    }
}
