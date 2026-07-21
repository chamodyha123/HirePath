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

        public PlatformAdminService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyResponseDto>>
            GetAllCompaniesAsync()
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

        public async Task<List<CompanyResponseDto>>
            GetPendingCompaniesAsync()
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

        public async Task<CompanyResponseDto?>
            GetCompanyByIdAsync(int id)
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

        public async Task<bool> RequestInformationAsync(
            int id,
            RequestInformationDto request)
        {
            var company =
                await _context.Companies.FindAsync(id);

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
            var company =
                await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            if (company.Status == CompanyStatus.Suspended)
            {
                throw new InvalidOperationException(
                    "Company is already suspended.");
            }

            company.Status = CompanyStatus.Suspended;
            company.SuspendedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateCompanyAsync(int id)
        {
            var company =
                await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return false;
            }

            if (company.Status == CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Company is already active.");
            }

            company.Status = CompanyStatus.Approved;
            company.SuspendedAt = null;
            company.ApprovedAt ??= DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PlatformDashboardDto>
            GetDashboardAsync()
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
                            CompanyStatus.Suspended),

                TotalUsers = await _context.Users.CountAsync(),
                TotalJobs = await _context.Jobs.CountAsync(),
                TotalApplications = await _context.JobApplications.CountAsync()
            };
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return false;
            }

            // Cascading clean up due to Restrict behavior
            if (company.Jobs != null && company.Jobs.Any())
            {
                var jobIds = company.Jobs.Select(j => j.Id).ToList();

                var jobApplications = await _context.JobApplications
                    .Where(ja => jobIds.Contains(ja.JobId))
                    .ToListAsync();
                _context.JobApplications.RemoveRange(jobApplications);

                var jobSkills = await _context.JobSkills
                    .Where(js => jobIds.Contains(js.JobId))
                    .ToListAsync();
                _context.JobSkills.RemoveRange(jobSkills);

                _context.Jobs.RemoveRange(company.Jobs);
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}