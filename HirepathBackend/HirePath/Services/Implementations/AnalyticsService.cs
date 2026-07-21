using HirePathAI.API.Data;
using HirePathAI.API.DTOs.PlatformAdmin.Analytics;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HirePathAI.API.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(
            ApplicationDbContext context,
            ILogger<AnalyticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AnalyticsResponseDto> GetAnalyticsDataAsync()
        {
            try
            {
                var response = new AnalyticsResponseDto();

                // Get company stats safely
                response.TotalCompanies = await _context.Companies.CountAsync();
                response.PendingCompanies = await _context.Companies.CountAsync(c => c.Status == CompanyStatus.Pending);
                response.ApprovedCompanies = await _context.Companies.CountAsync(c => c.Status == CompanyStatus.Approved || c.Status == CompanyStatus.Active);
                response.SuspendedCompanies = await _context.Companies.CountAsync(c => c.Status == CompanyStatus.Suspended);
                
                // Get platform stats
                response.TotalUsers = await _context.Users.CountAsync();
                response.TotalJobs = await _context.Jobs.CountAsync();
                response.TotalApplications = await _context.JobApplications.CountAsync();

                // Calculate rates
                var totalApps = response.TotalApplications > 0 ? response.TotalApplications : 1;
                var hiredCount = await _context.JobApplications.CountAsync(a => a.Status == ApplicationStatus.Hired);
                var interviewCount = await _context.JobApplications.CountAsync(a => a.Status == ApplicationStatus.Interviewed);

                response.JobSuccessRate = Math.Round((decimal)hiredCount / totalApps * 100, 1);
                response.InterviewConversionRate = Math.Round((decimal)interviewCount / totalApps * 100, 1);
                response.AverageScreeningTime = "2.5 days";

                // Role Distribution
                var userRoles = await _context.UserRoles.ToListAsync();
                var roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name ?? "Unknown");
                var totalUsers = response.TotalUsers > 0 ? response.TotalUsers : 1;

                var roleDistribution = userRoles
                    .GroupBy(ur => ur.RoleId)
                    .Select(g => new RoleDistributionDto
                    {
                        Name = roles.ContainsKey(g.Key) ? roles[g.Key] : "Unknown",
                        Percentage = Math.Round((decimal)g.Count() / totalUsers * 100, 1),
                        Color = GetColorForRole(roles.ContainsKey(g.Key) ? roles[g.Key] : "Unknown")
                    })
                    .ToList();

                response.RoleDistribution = roleDistribution;

                // Top Skills
                var topSkills = await _context.JobSkills
                    .GroupBy(js => js.SkillName)
                    .Select(g => new TopSkillDto
                    {
                        Name = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                response.TopSkills = topSkills;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics data");
                throw;
            }
        }

        private string GetColorForRole(string roleName)
        {
            return roleName?.ToLower() switch
            {
                "candidate" => "#3b82f6",
                "recruiter" => "#10b981",
                "hiringmanager" => "#f59e0b",
                "companyadmin" => "#8b5cf6",
                "admin" => "#ef4444",
                "superadmin" => "#ec4899",
                _ => "#6b7280"
            };
        }
    }
}
