using HirePathAI.DTOs;

namespace HirePathAI.Services
{
    public interface IRecruiterService
    {
        Task<CompanyResponseDTO> AddCompanyAsync(CompanyCreateDTO dto);
        Task<IEnumerable<CompanyResponseDTO>> GetAllCompaniesAsync();
        Task<DepartmentResponseDTO> AddDepartmentAsync(DepartmentCreateDTO dto);
        Task<IEnumerable<DepartmentResponseDTO>> GetDepartmentsAsync(int companyId);
        Task<JobResponseDTO> PostJobAsync(JobCreateDTO dto);
        Task<JobResponseDTO?> GetJobDetailsAsync(int id);
        Task<IEnumerable<JobResponseDTO>> SearchJobsAsync(string? search, string? location);
        Task<JobResponseDTO?> EditJobAsync(int id, JobCreateDTO dto);
        Task<bool> RemoveJobAsync(int id);
        Task<object> GetDashboardStatsAsync(int? companyId);
    }
}