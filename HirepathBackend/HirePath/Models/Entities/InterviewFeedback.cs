using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class InterviewFeedback : BaseEntity
    {
        public int InterviewId { get; set; }
        public Interview? Interview { get; set; }

        public int ApplicationId { get; set; }
        public JobApplication? Application { get; set; }

        public int EvaluatorId { get; set; }
        public User? Evaluator { get; set; }

        // Scores (0-100)
        public decimal? TechnicalScore { get; set; }
        public decimal? CommunicationScore { get; set; }
        public decimal? ProblemSolvingScore { get; set; }
        public decimal? CulturalFitScore { get; set; }
        public decimal? OverallScore { get; set; }

        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }

        public HiringRecommendation Recommendation { get; set; } = HiringRecommendation.Pending;
        public DateTime FeedbackDate { get; set; } = DateTime.UtcNow;
        public bool IsSubmitted { get; set; }
    }

    public enum HiringRecommendation
    {
        Pending = 0,
        Hire = 1,
        Hold = 2,
        Reject = 3,
        StrongHire = 4
    }
}