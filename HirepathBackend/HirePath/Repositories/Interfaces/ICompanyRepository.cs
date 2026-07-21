using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company> AddCompanyAsync(Company entity) => throw new NotImplementedException();
        Task<Company?> GetCompanyWithDetailsAsync(int id) => Task.FromResult<Company?>(null);
        Task<IEnumerable<Company>> GetCompaniesWithUsersAsync() => Task.FromResult<IEnumerable<Company>>(Array.Empty<Company>());
        Task<bool> CompanyExistsAsync(int id) => Task.FromResult(false);
    }
}