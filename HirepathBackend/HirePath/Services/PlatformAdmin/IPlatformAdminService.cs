using HirePathAI.API.DTOs.PlatformAdmin.Companies;
using HirePathAI.API.DTOs.PlatformAdmin.Dashboard;

namespace HirePathAI.API.Services.PlatformAdmin
{
    public interface IPlatformAdminService
    {
        Task<List<CompanyResponseDto>>
            GetAllCompaniesAsync();

        Task<List<CompanyResponseDto>>
            GetPendingCompaniesAsync();

        Task<CompanyResponseDto?>
            GetCompanyByIdAsync(int id);

        Task<bool>
            ApproveCompanyAsync(
                int id,
                ApproveCompanyDto request);

        Task<bool>
            RejectCompanyAsync(
                int id,
                RejectCompanyDto request);

        Task<bool>
            RequestInformationAsync(
                int id,
                RequestInformationDto request);

        Task<bool>
            SuspendCompanyAsync(int id);

        Task<bool>
            ActivateCompanyAsync(int id);

        Task<PlatformDashboardDto>
            GetDashboardAsync();
    }
}