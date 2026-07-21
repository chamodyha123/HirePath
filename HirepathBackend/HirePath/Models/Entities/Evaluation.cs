using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Evaluation : BaseEntity
    {
        public int ApplicationId { get; set; }
        public JobApplication? Application { get; set; }

        public int EvaluatorId { get; set; }
        public User? Evaluator { get; set; }

        // Individual Scores (0-100)
        public decimal? ResumeScore { get; set; }
        public decimal? AIScore { get; set; }
        public decimal? InterviewScore { get; set; }
        public decimal? HiringManagerScore { get; set; }
        public decimal? OverallScore { get; set; }

        public string? Comments { get; set; }
        public string? Recommendations { get; set; }

        public bool IsFinalized { get; set; }
        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    }
}