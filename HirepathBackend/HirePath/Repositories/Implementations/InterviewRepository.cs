using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly ApplicationDbContext _context;

        public InterviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            return await _context.Interviews
                .Include(i => i.JobApplication)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Loads JobApplication -> Job -> Company so services can
        // verify the interview belongs to the caller's company.
        public async Task<Interview?> GetByIdWithCompanyAsync(int id)
        {
            return await _context.Interviews
                .Include(i => i.JobApplication)
                    .ThenInclude(a => a!.Job)
                        .ThenInclude(j => j!.Company)
                .Include(i => i.JobApplication)
                    .ThenInclude(a => a!.CandidateProfile)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Interviews
                .Where(i => i.JobApplicationId == applicationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interview>> GetByCompanyAsync(int companyId)
        {
            return await _context.Interviews
                .Include(i => i.JobApplication)
                    .ThenInclude(a => a!.Job)
                .Where(i => i.JobApplication!.Job!.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interview>> GetAllAsync()
        {
            return await _context.Interviews
                .Include(i => i.JobApplication)
                    .ThenInclude(a => a!.Job)
                .ToListAsync();
        }

        public void Update(Interview interview)
        {
            _context.Interviews.Update(interview);
        }

        public void Delete(Interview interview)
        {
            _context.Interviews.Remove(interview);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}