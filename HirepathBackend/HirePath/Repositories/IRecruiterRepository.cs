using HirePathAI.API.Models.Entities;

namespace HirePathAI.Repositories
{
    public interface IRecruiterRepository
    {
        Task<Company> CreateCompanyAsync(Company company);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Department> CreateDepartmentAsync(Department department);
        Task<IEnumerable<Department>> GetDepartmentsByCompanyAsync(int companyId);
        Task<Job> CreateJobAsync(Job job);
        Task<Job?> GetJobByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllJobsAsync(string? search, string? location);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(int id);
        Task AddJobSkillsAsync(IEnumerable<JobSkill> skills);
    }
}