using System.Collections.Generic;
using System.Threading.Tasks;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> CreateAsync(Company company);
        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetCompanyWithDetailsAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<IEnumerable<Company>> GetCompaniesWithUsersAsync();
        Task<Company> UpdateAsync(Company company);
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignUserToCompanyAsync(int userId, int companyId);
        Task<bool> RemoveUserFromCompanyAsync(int userId);
        Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId);
        Task<bool> CompanyExistsAsync(int id);
        Task<bool> IsUserInCompanyAsync(int userId, int companyId);
    }
}
