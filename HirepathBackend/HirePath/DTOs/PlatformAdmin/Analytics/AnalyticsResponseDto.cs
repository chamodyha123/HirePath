// HirePathAI.API/DTOs/PlatformAdmin/Analytics/AnalyticsResponseDto.cs
namespace HirePathAI.API.DTOs.PlatformAdmin.Analytics
{
    public class AnalyticsResponseDto
    {
        public decimal JobSuccessRate { get; set; }
        public decimal InterviewConversionRate { get; set; }
        public string AverageScreeningTime { get; set; } = string.Empty;
        public List<RoleDistributionDto> RoleDistribution { get; set; } = new();
        public List<TopSkillDto> TopSkills { get; set; } = new();

        // Company Stats
        public int TotalCompanies { get; set; }
        public int PendingCompanies { get; set; }
        public int ApprovedCompanies { get; set; }
        public int SuspendedCompanies { get; set; }

        // Platform Stats
        public int TotalUsers { get; set; }
        public int TotalJobs { get; set; }
        public int TotalApplications { get; set; }
    }

    public class RoleDistributionDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class TopSkillDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}