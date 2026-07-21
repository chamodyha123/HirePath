using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;

        public CompanyService(ICompanyRepository companyRepository, IUserRepository userRepository)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }

        public async Task<Company> CreateAsync(Company company)
        {
            company.CreatedAt = DateTime.UtcNow;
            await _companyRepository.AddAsync(company);
            await _companyRepository.SaveChangesAsync();
            return company;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _companyRepository.GetByIdAsync(id);
        }

        public async Task<Company?> GetCompanyWithDetailsAsync(int id)
        {
            return await _companyRepository.GetCompanyWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithUsersAsync()
        {
            return await _companyRepository.GetCompaniesWithUsersAsync();
        }

        public async Task<Company> UpdateAsync(Company company)
        {
            var existing = await _companyRepository.GetByIdAsync(company.Id);
            if (existing == null)
                throw new Exception("Company not found");

            existing.Name = company.Name;
            existing.Description = company.Description;
            existing.Website = company.Website;
            existing.Location = company.Location;
            existing.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(existing);
            await _companyRepository.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return false;

            await _companyRepository.DeleteAsync(company);
            await _companyRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignUserToCompanyAsync(int userId, int companyId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
                return false;

            user.CompanyId = companyId;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserFromCompanyAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.CompanyId = null;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId)
        {
            return await _userRepository.GetUsersByCompanyAsync(companyId);
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _companyRepository.CompanyExistsAsync(id);
        }

        public async Task<bool> IsUserInCompanyAsync(int userId, int companyId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.CompanyId == companyId;
        }
    }
}