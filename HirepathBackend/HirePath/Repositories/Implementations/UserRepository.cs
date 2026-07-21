using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HirePathAI.API.Repositories.Implementations
{
    public class UserRepository
        : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }
        public async Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Include(u => u.Company)
                .ToListAsync();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<int?> GetUserCompanyIdAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.CompanyId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            var roleNames = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => r.Name)
                .ToListAsync();

            return roleNames.Where(n => n != null).Select(n => n!);
        }
    }
}
