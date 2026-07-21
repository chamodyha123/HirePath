// HirePathAI.API/DTOs/AI/AIAnalyticsDtos.cs
namespace HirePathAI.API.DTOs.AI
{
    // ============ RESUME PARSING ============
    public class ResumeParseRequestDto
    {
        public string ResumeText { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }

    public class ResumeParseResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public ResumeAnalysisResultDto? Data { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    public class ResumeAnalysisResultDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<SkillDetailDto> SkillDetails { get; set; } = new();
        public int YearsOfExperience { get; set; }
        public string? Summary { get; set; }
        public List<EducationExtractedDto> Education { get; set; } = new();
        public List<ExperienceExtractedDto> Experience { get; set; } = new();
        public List<string> Certifications { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    public class SkillDetailDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class EducationExtractedDto
    {
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Grade { get; set; }
    }

    public class ExperienceExtractedDto
    {
        public string Company { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<string> Responsibilities { get; set; } = new();
    }

    // ============ JOB MATCHING & RANKING ============
    public class MatchRequestDto
    {
        public int JobId { get; set; }
        public int? CandidateId { get; set; }
        public string? JobDescription { get; set; }
        public string? CandidateProfile { get; set; }
    }

    public class MatchResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public JobMatchResultDto? Data { get; set; }
    }

    public class JobMatchResultDto
    {
        public int JobId { get; set; }
        public int? CandidateId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public decimal OverallMatchScore { get; set; }
        public decimal SkillMatchScore { get; set; }
        public decimal ExperienceMatchScore { get; set; }
        public decimal EducationMatchScore { get; set; }
        public decimal LocationMatchScore { get; set; }
        public List<string> MatchedSkills { get; set; } = new();
        public List<string> MissingSkills { get; set; } = new();
        public List<string> MatchedExperience { get; set; } = new();
        public List<string> MissingExperience { get; set; } = new();
        public string MatchLevel { get; set; } = string.Empty;
        public Dictionary<string, decimal> ScoreBreakdown { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public DateTime MatchedAt { get; set; }
    }

    public class RankRequestDto
    {
        public int JobId { get; set; }
        public List<int> CandidateIds { get; set; } = new();
        public bool IncludeDetails { get; set; } = true;
    }

    public class RankResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<JobMatchResultDto> Candidates { get; set; } = new();
        public RankingSummaryDto Summary { get; set; } = new();
    }

    public class RankingSummaryDto
    {
        public int TotalCandidates { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public List<string> TopSkills { get; set; } = new();
        public List<string> CommonMissingSkills { get; set; } = new();
        public Dictionary<string, int> ScoreDistribution { get; set; } = new();
    }

    // ============ SKILL EXTRACTION ============
    public class SkillExtractionResultDto
    {
        public List<ExtractedSkillDto> Skills { get; set; } = new();
        public List<ExtractedSkillDto> PrimarySkills { get; set; } = new();
        public int TotalSkills { get; set; }
        public double ConfidenceScore { get; set; }
    }

    public class ExtractedSkillDto
    {
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = "Intermediate";
        public double ConfidenceScore { get; set; }
        public int Frequency { get; set; }
    }

    // ============ JOB RECOMMENDATIONS ============
    public class JobRecommendationRequestDto
    {
        public int CandidateId { get; set; }
        public int? Limit { get; set; } = 10;
        public bool IncludeApplied { get; set; } = false;
        public string? Location { get; set; }
        public List<string>? PreferredSkills { get; set; }
    }

    public class JobRecommendationResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<JobRecommendationDto> Recommendations { get; set; } = new();
        public RecommendationSummaryDto Summary { get; set; } = new();
    }

    public class JobRecommendationDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal MatchScore { get; set; }
        public string MatchReason { get; set; } = string.Empty;
        public List<string> WhyThisJob { get; set; } = new();
        public List<string> SkillsToImprove { get; set; } = new();
        public bool IsApplied { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime PostedDate { get; set; }
    }

    public class RecommendationSummaryDto
    {
        public int TotalRecommendations { get; set; }
        public decimal AverageMatchScore { get; set; }
        public decimal HighestMatchScore { get; set; }
        public Dictionary<string, int> JobsByLocation { get; set; } = new();
        public List<string> TopRecommendedSkills { get; set; } = new();
    }

    // ============ RECRUITMENT ANALYTICS ============
    public class RecruitmentAnalyticsRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public string? TimePeriod { get; set; }
    }

    public class RecruitmentAnalyticsResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public OverviewAnalyticsDto Overview { get; set; } = new();
        public PipelineAnalyticsDto Pipeline { get; set; } = new();
        public QualityAnalyticsDto Quality { get; set; } = new();
        public SourceAnalyticsDto Sources { get; set; } = new();
        public TimeAnalyticsDto TimeMetrics { get; set; } = new();
        public CostAnalyticsDto Costs { get; set; } = new();
        public List<TrendAnalyticsDto> Trends { get; set; } = new();
        public PredictiveAnalyticsDto Predictions { get; set; } = new();
    }

    public class OverviewAnalyticsDto
    {
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int NewApplications { get; set; }
        public int InterviewsScheduled { get; set; }
        public int InterviewsCompleted { get; set; }
        public int OffersMade { get; set; }
        public int OffersAccepted { get; set; }
        public int HiredCount { get; set; }
        public decimal ApplicationToHireRate { get; set; }
        public decimal InterviewToHireRate { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class PipelineAnalyticsDto
    {
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
        public Dictionary<string, int> StageDropoff { get; set; } = new();
        public Dictionary<string, decimal> AverageTimeInStage { get; set; } = new();
        public List<PipelineStageDto> PipelineStages { get; set; } = new();
    }

    public class PipelineStageDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal ConversionRate { get; set; }
        public double AverageTimeDays { get; set; }
    }

    public class QualityAnalyticsDto
    {
        public decimal AverageMatchScore { get; set; }
        public decimal MedianMatchScore { get; set; }
        public Dictionary<string, decimal> ScoresByJob { get; set; } = new();
        public Dictionary<string, int> TopSkills { get; set; } = new();
        public Dictionary<string, int> TopCertifications { get; set; } = new();
        public Dictionary<string, int> CandidateQualityDistribution { get; set; } = new();
        public decimal QualityScoreTrend { get; set; }
    }

    public class SourceAnalyticsDto
    {
        public Dictionary<string, int> ApplicationsBySource { get; set; } = new();
        public Dictionary<string, int> HiresBySource { get; set; } = new();
        public Dictionary<string, decimal> ConversionBySource { get; set; } = new();
        public Dictionary<string, decimal> CostPerHireBySource { get; set; } = new();
        public List<SourcePerformanceDto> TopSources { get; set; } = new();
    }

    public class SourcePerformanceDto
    {
        public string Source { get; set; } = string.Empty;
        public int Applications { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CostPerHire { get; set; }
    }

    public class TimeAnalyticsDto
    {
        public double AverageDaysToHire { get; set; }
        public double MedianDaysToHire { get; set; }
        public double MaxDaysToHire { get; set; }
        public double MinDaysToHire { get; set; }
        public Dictionary<string, double> TimeByStage { get; set; } = new();
        public Dictionary<string, double> TimeByDepartment { get; set; } = new();
        public Dictionary<string, double> TimeByJobLevel { get; set; } = new();
        public List<TimeTrendDto> TimeTrends { get; set; } = new();
    }

    public class TimeTrendDto
    {
        public DateTime Period { get; set; }
        public double AverageTimeToHire { get; set; }
        public int HiresCount { get; set; }
    }

    public class CostAnalyticsDto
    {
        public decimal AverageCostPerHire { get; set; }
        public decimal TotalRecruitmentCost { get; set; }
        public Dictionary<string, decimal> CostBySource { get; set; } = new();
        public Dictionary<string, decimal> CostByDepartment { get; set; } = new();
        public Dictionary<string, decimal> CostByJobLevel { get; set; } = new();
        public decimal CostTrend { get; set; }
    }

    public class TrendAnalyticsDto
    {
        public DateTime Date { get; set; }
        public int Applications { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }
        public decimal AverageMatchScore { get; set; }
        public int ActiveJobs { get; set; }
    }

    public class PredictiveAnalyticsDto
    {
        public int PredictedHiresNextMonth { get; set; }
        public int PredictedApplicationsNextMonth { get; set; }
        public Dictionary<string, int> PredictedSkillsDemand { get; set; } = new();
        public Dictionary<string, int> PredictedHiresByDepartment { get; set; } = new();
        public Dictionary<string, decimal> SuccessProbability { get; set; } = new();
    }

    // ============ AI REPORTING ============
    public class AIReportRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyId { get; set; }
        public string ReportType { get; set; } = "comprehensive";
        public string Format { get; set; } = "json";
        public bool IncludeDetails { get; set; } = true;
    }

    public class AIReportResponseDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public ReportDataDto Report { get; set; } = new();
        public byte[]? FileContent { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }

    public class ReportDataDto
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public ExecutiveSummaryDto ExecutiveSummary { get; set; } = new();
        public List<KeyMetricDto> KeyMetrics { get; set; } = new();
        public List<ChartDataDto> Charts { get; set; } = new();
        public List<InsightDto> Insights { get; set; } = new();
        public List<RecommendationDto> Recommendations { get; set; } = new();
        public AnomalyDetectionDto Anomalies { get; set; } = new();
    }

    public class ExecutiveSummaryDto
    {
        public string Overview { get; set; } = string.Empty;
        public List<string> KeyHighlights { get; set; } = new();
        public List<string> AreasForImprovement { get; set; } = new();
        public string OverallHealth { get; set; } = "Good";
    }

    public class KeyMetricDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Trend { get; set; } = "stable";
        public decimal TrendValue { get; set; }
        public string Status { get; set; } = "neutral";
    }

    public class ChartDataDto
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new();
        public List<ChartDatasetDto> Datasets { get; set; } = new();
    }

    public class ChartDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public List<decimal> Data { get; set; } = new();
        public string Color { get; set; } = string.Empty;
    }

    public class InsightDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "info";
        public string Category { get; set; } = string.Empty;
        public List<string> RelatedMetrics { get; set; } = new();
    }

    public class RecommendationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "medium";
        public decimal Impact { get; set; }
        public decimal Effort { get; set; }
        public List<string> Steps { get; set; } = new();
    }

    public class AnomalyDetectionDto
    {
        public List<AnomalyDto> DetectedAnomalies { get; set; } = new();
        public bool HasAnomalies { get; set; }
    }

    public class AnomalyDto
    {
        public string Metric { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Deviation { get; set; }
        public string Severity { get; set; } = "medium";
    }
}