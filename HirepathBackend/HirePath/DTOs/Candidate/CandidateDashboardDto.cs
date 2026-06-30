namespace HirePathAI.API.DTOs.Candidate 
{
    public class CandidateDashboardDto
    {
        public int TotalApplications { get; set; }
        public int Shortlisted { get; set; }
        public int Interviews { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }

        public List<RecentApplicationDto> RecentApplications { get; set; } = new();
        public List<JobRecommendationDto> Recommendations { get; set; } = new();
    }

    public class RecentApplicationDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
        public double? MatchScore { get; set; }
    }

    public class JobRecommendationDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double MatchScore { get; set; }
        public List<string> MatchingSkills { get; set; } = new();
    }
}