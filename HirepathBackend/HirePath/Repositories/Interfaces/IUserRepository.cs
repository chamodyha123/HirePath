using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);

        Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId);
        // Return the roles for a given user
        Task<IEnumerable<string>> GetRolesAsync(User user);

        // Return the company id associated with the user (or null)
        Task<int?> GetUserCompanyIdAsync(int userId);
    }
}


