using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> CreateAsync(string name, string? description, string? website, string? location);
        Task<Company?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<bool> AssignUserToCompanyAsync(int userId, int companyId);
    }
}