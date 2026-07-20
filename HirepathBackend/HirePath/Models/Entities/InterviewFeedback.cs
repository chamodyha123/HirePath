using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class InterviewFeedback : BaseEntity
    {
        public int InterviewId { get; set; }
        public Interview? Interview { get; set; }

        // The Hiring Manager who submitted this feedback (audit trail)
        public int SubmittedByUserId { get; set; }
        public User? SubmittedByUser { get; set; }

        public int TechnicalScore { get; set; }
        public int CommunicationScore { get; set; }
        public int ProblemSolvingScore { get; set; }

        public string? Comments { get; set; }
        public RecommendationType Recommendation { get; set; }
    }
}