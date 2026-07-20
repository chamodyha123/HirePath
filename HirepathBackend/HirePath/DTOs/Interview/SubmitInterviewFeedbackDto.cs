using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Interview
{
    public class SubmitInterviewFeedbackDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "InterviewId must be greater than 0.")]
        public int InterviewId { get; set; }

        [Range(1, 10, ErrorMessage = "TechnicalScore must be between 1 and 10.")]
        public int TechnicalScore { get; set; }

        [Range(1, 10, ErrorMessage = "CommunicationScore must be between 1 and 10.")]
        public int CommunicationScore { get; set; }

        [Range(1, 10, ErrorMessage = "ProblemSolvingScore must be between 1 and 10.")]
        public int ProblemSolvingScore { get; set; }

        [StringLength(4000)]
        public string? Comments { get; set; }

        // Expected values: "Hire", "Hold", "Reject"
        [Required]
        [StringLength(20)]
        public string Recommendation { get; set; } = string.Empty;
    }
}