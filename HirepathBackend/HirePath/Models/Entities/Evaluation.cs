using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Evaluation : BaseEntity
    {
        public int JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        public decimal ResumeScore { get; set; }
        public decimal AIScore { get; set; }
        public decimal InterviewScore { get; set; }
        public decimal OverallScore { get; set; }

        // Who triggered/finalized this evaluation (audit trail)
        public int EvaluatedByUserId { get; set; }
        public User? EvaluatedByUser { get; set; }
    }
}