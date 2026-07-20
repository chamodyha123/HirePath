using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.Repositories
{
    public class RecruiterRepository : IRecruiterRepository
    {
        private readonly ApplicationDbContext _context;

        public RecruiterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            await _context.Set<Company>().AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Set<Company>().ToListAsync();
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            await _context.Set<Department>().AddAsync(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<IEnumerable<Department>> GetDepartmentsByCompanyAsync(int companyId)
        {
            return await _context.Set<Department>().Where(d => d.CompanyId == companyId).ToListAsync();
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            await _context.Set<Job>().AddAsync(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Set<Job>()
                .Include(j => j.Company)
                .Include(j => j.Department)
                .Include(j => j.RequiredSkills)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync(string? search, string? location)
        {
            var query = _context.Set<Job>()
                .Include(j => j.Company)
                .Include(j => j.Department)
                .Include(j => j.RequiredSkills)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(j => j.Location.Contains(location));
            }

            return await query.ToListAsync();
        }

        public async Task<Job> UpdateJobAsync(Job job)
        {
            _context.Set<Job>().Update(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _context.Set<Job>().FindAsync(id);
            if (job == null) return false;

            _context.Set<Job>().Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddJobSkillsAsync(IEnumerable<JobSkill> skills)
        {
            await _context.Set<JobSkill>().AddRangeAsync(skills);
            await _context.SaveChangesAsync();
        }
    }
}