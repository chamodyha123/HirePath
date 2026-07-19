using HirePathAI.API.Data;
using HirePathAI.API.DTOs.PlatformAdmin.Companies;
using HirePathAI.API.DTOs.PlatformAdmin.Dashboard;
using HirePathAI.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Services.PlatformAdmin
{
    public class PlatformAdminService : IPlatformAdminService
    {
        private readonly ApplicationDbContext _context;

        public PlatformAdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Select(company => new CompanyResponseDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Website = company.Website,
                    Location = company.Location,
                    Industry = company.Industry,
                    BusinessRegistrationNumber =
                        company.BusinessRegistrationNumber,

                    CompanyEmail = company.Email,

                    Phone = company.Phone,
                    Address = company.Address,
                    RepresentativeName =
                        company.RepresentativeName,
                    RepresentativeEmail =
                        company.RepresentativeEmail,
                    Status = company.Status.ToString(),
                    ApprovedAt = company.ApprovedAt,
                    RejectedAt = company.RejectedAt,
                    SuspendedAt = company.SuspendedAt,
                    RejectionReason =
                        company.RejectionReason,
                    AdminNotes = company.AdminNotes
                })
                .ToListAsync();
        }

        public async Task<List<CompanyResponseDto>> GetPendingCompaniesAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(company =>
                    company.Status == CompanyStatus.Pending)
                .Select(company => new CompanyResponseDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Website = company.Website,
                    Location = company.Location,
                    Industry = company.Industry,
                    BusinessRegistrationNumber =
                        company.BusinessRegistrationNumber,

                    CompanyEmail = company.Email,

                    Phone = company.Phone,
                    Address = company.Address,
                    RepresentativeName =
                        company.RepresentativeName,
                    RepresentativeEmail =
                        company.RepresentativeEmail,
                    Status = company.Status.ToString(),
                    ApprovedAt = company.ApprovedAt,
                    RejectedAt = company.RejectedAt,
                    SuspendedAt = company.SuspendedAt,
                    RejectionReason =
                        company.RejectionReason,
                    AdminNotes = company.AdminNotes
                })
                .ToListAsync();
        }

        public async Task<CompanyResponseDto?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(company => company.Id == id)
                .Select(company => new CompanyResponseDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Website = company.Website,
                    Location = company.Location,
                    Industry = company.Industry,
                    BusinessRegistrationNumber =
                        company.BusinessRegistrationNumber,

                    CompanyEmail = company.Email,

                    Phone = company.Phone,
                    Address = company.Address,
                    RepresentativeName =
                        company.RepresentativeName,
                    RepresentativeEmail =
                        company.RepresentativeEmail,
                    Status = company.Status.ToString(),
                    ApprovedAt = company.ApprovedAt,
                    RejectedAt = company.RejectedAt,
                    SuspendedAt = company.SuspendedAt,
                    RejectionReason =
                        company.RejectionReason,
                    AdminNotes = company.AdminNotes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ApproveCompanyAsync(
            int id,
            ApproveCompanyDto request)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            company.Status = CompanyStatus.Approved;
            company.ApprovedAt = DateTime.UtcNow;
            company.RejectedAt = null;
            company.SuspendedAt = null;
            company.RejectionReason = null;
            company.AdminNotes = request.AdminNotes;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectCompanyAsync(
            int id,
            RejectCompanyDto request)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            company.Status = CompanyStatus.Rejected;
            company.RejectedAt = DateTime.UtcNow;
            company.ApprovedAt = null;
            company.SuspendedAt = null;
            company.RejectionReason = request.RejectionReason;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RequestInformationAsync(
            int id,
            RequestInformationDto request)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            company.Status =
                CompanyStatus.MoreInformationRequired;

            company.AdminNotes = request.Message;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SuspendCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            company.Status = CompanyStatus.Suspended;
            company.SuspendedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            company.Status = CompanyStatus.Approved;
            company.SuspendedAt = null;
            company.ApprovedAt ??= DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PlatformDashboardDto> GetDashboardAsync()
        {
            return new PlatformDashboardDto
            {
                TotalCompanies =
                    await _context.Companies.CountAsync(),

                PendingCompanies =
                    await _context.Companies.CountAsync(
                        company =>
                            company.Status ==
                            CompanyStatus.Pending),

                ApprovedCompanies =
                    await _context.Companies.CountAsync(
                        company =>
                            company.Status ==
                            CompanyStatus.Approved),

                RejectedCompanies =
                    await _context.Companies.CountAsync(
                        company =>
                            company.Status ==
                            CompanyStatus.Rejected),

                SuspendedCompanies =
                    await _context.Companies.CountAsync(
                        company =>
                            company.Status ==
                            CompanyStatus.Suspended)
            };
        }
    }
}