using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Interview
{
    public class SubmitInterviewFeedbackDto
    {
        [Range(1, int.MaxValue)]
        public int InterviewId { get; set; }

        [Range(1, 10)]
        public int TechnicalScore { get; set; }

        [Range(1, 10)]
        public int CommunicationScore { get; set; }

        [Range(1, 10)]
        public int ProblemSolvingScore { get; set; }

        [StringLength(4000)]
        public string? Comments { get; set; }

        [Required, StringLength(20)]
        public string Recommendation { get; set; } = string.Empty;
    }
}
