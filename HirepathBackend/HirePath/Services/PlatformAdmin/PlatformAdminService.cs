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
                CompanyStatus.Pending;

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
                            CompanyStatus.Suspended),

                TotalUsers = await _context.Users.CountAsync(),
                TotalJobs = await _context.Jobs.CountAsync(),
                TotalApplications = await _context.JobApplications.CountAsync()
            };
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return false;

            // Several relationships in ApplicationDbContext intentionally use
            // Restrict/NoAction, so a company cannot be deleted until dependent
            // workflow/audit rows are removed or detached explicitly.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var jobIds = await _context.Jobs
                    .Where(j => j.CompanyId == id)
                    .Select(j => j.Id)
                    .ToListAsync();

                var applicationIds = await _context.JobApplications
                    .Where(a => jobIds.Contains(a.JobId))
                    .Select(a => a.Id)
                    .ToListAsync();

                var interviewIds = await _context.Interviews
                    .Where(i => applicationIds.Contains(i.JobApplicationId))
                    .Select(i => i.Id)
                    .ToListAsync();

                // Remove workflow rows first. This also avoids Restrict FKs from
                // JobApplication -> Job/Resume/CandidateProfile blocking deletion.
                var feedback = await _context.InterviewFeedbacks
                    .Where(f => interviewIds.Contains(f.InterviewId))
                    .ToListAsync();
                _context.InterviewFeedbacks.RemoveRange(feedback);

                var interviews = await _context.Interviews
                    .Where(i => applicationIds.Contains(i.JobApplicationId))
                    .ToListAsync();
                _context.Interviews.RemoveRange(interviews);

                var evaluations = await _context.Evaluations
                    .Where(e => applicationIds.Contains(e.JobApplicationId))
                    .ToListAsync();
                _context.Evaluations.RemoveRange(evaluations);

                var statusHistory = await _context.ApplicationStatusHistories
                    .Where(h => applicationIds.Contains(h.JobApplicationId))
                    .ToListAsync();
                _context.ApplicationStatusHistories.RemoveRange(statusHistory);

                var applications = await _context.JobApplications
                    .Where(a => applicationIds.Contains(a.Id))
                    .ToListAsync();
                _context.JobApplications.RemoveRange(applications);

                var jobSkills = await _context.JobSkills
                    .Where(s => jobIds.Contains(s.JobId))
                    .ToListAsync();
                _context.JobSkills.RemoveRange(jobSkills);

                var jobs = await _context.Jobs
                    .Where(j => j.CompanyId == id)
                    .ToListAsync();
                _context.Jobs.RemoveRange(jobs);

                // CompanyRegistrationRequest.CreatedCompanyId uses NoAction.
                // Preserve the registration audit record but detach it from the
                // company being permanently deleted.
                var registrationRequests = await _context.CompanyRegistrationRequests
                    .Where(r => r.CreatedCompanyId == id)
                    .ToListAsync();
                foreach (var request in registrationRequests)
                {
                    request.CreatedCompanyId = null;
                    request.UpdatedAt = DateTime.UtcNow;
                }

                // User.CompanyId uses Restrict. Keep user accounts for audit/login
                // management, but detach them from the deleted company. CompanyMember
                // rows and invitations are configured to cascade from Company.
                var companyUsers = await _context.Users
                    .Where(u => u.CompanyId == id)
                    .ToListAsync();
                foreach (var user in companyUsers)
                    user.CompanyId = null;

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
