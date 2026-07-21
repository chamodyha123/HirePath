namespace HirePathAI.API.DTOs.PlatformAdmin.Analytics
{
    public class AnalyticsResponseDto
    {
        public int JobSuccessRate { get; set; }
        public int InterviewConversionRate { get; set; }
        public string AverageScreeningTime { get; set; } = "1.2 Days";
        public List<RoleDistributionDto> RoleDistribution { get; set; } = new();
        public List<TopSkillDto> TopSkills { get; set; } = new();
    }

    public class RoleDistributionDto
    {
        public string Name { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public string Color { get; set; } = "var(--blue)";
    }

    public class TopSkillDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
