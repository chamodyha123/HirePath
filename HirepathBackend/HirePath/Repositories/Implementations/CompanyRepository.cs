using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Repositories.Implementations
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetCompanyWithDetailsAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Jobs)
                .Include(c => c.Departments)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithUsersAsync()
        {
            return await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Jobs)
                .Include(c => c.Departments)
                .ToListAsync();
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _context.Companies.AnyAsync(c => c.Id == id);
        }

        public async Task<Company> AddCompanyAsync(Company entity)
        {
            await AddAsync(entity);            // GenericRepository.AddAsync(T)
            await SaveChangesAsync();         // GenericRepository.SaveChangesAsync()
            return entity;
        }
    }
}