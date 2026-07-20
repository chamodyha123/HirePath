using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Implementations;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IUserRepository _userRepo;

        public CompanyService(ICompanyRepository companyRepo, IUserRepository userRepo)
        {
            _companyRepo = companyRepo;
            _userRepo = userRepo;
        }

        public async Task<Company> CreateAsync(string name, string? description, string? website, string? location)
        {
            var company = new Company
            {
                Name = name,
                Description = description,
                Website = website,
                Location = location
            };

            await _companyRepo.AddAsync(company);
            await _companyRepo.SaveChangesAsync();

            return company;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _companyRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _companyRepo.GetAllAsync();
        }

        public async Task<bool> AssignUserToCompanyAsync(int userId, int companyId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return false;

            var company = await _companyRepo.GetByIdAsync(companyId);
            if (company == null)
                return false;

            user.CompanyId = companyId;
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return true;
        }
    }
}