namespace HirePathAI.API.Configuration
{
    public class AISettings
    {
        public SovrenSettings Sovren { get; set; } = new();
        public RankingSettings Ranking { get; set; } = new();
        public AnalyticsSettings Analytics { get; set; } = new();
    }

    public class SovrenSettings
    {
        public string AccountId { get; set; } = string.Empty;
        public string ServiceKey { get; set; } = string.Empty;
        public string DataCenter { get; set; } = "US";
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class RankingSettings
    {
        public decimal SkillWeight { get; set; } = 0.4m;
        public decimal ExperienceWeight { get; set; } = 0.3m;
        public decimal EducationWeight { get; set; } = 0.2m;
        public decimal LocationWeight { get; set; } = 0.1m;
        public int MinMatchPercentage { get; set; } = 60;
    }

    public class AnalyticsSettings
    {
        public int CacheDurationMinutes { get; set; } = 5;
        public int TopSkillsCount { get; set; } = 10;
        public int TopCandidatesCount { get; set; } = 20;
        public bool EnableRealTimeAnalytics { get; set; } = true;
    }
}
