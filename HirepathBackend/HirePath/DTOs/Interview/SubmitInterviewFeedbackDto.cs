namespace HirePathAI.API.DTOs.Interview
{
    public class SubmitInterviewFeedbackDto
    {
        public int InterviewId { get; set; }
        public int TechnicalScore { get; set; }
        public int CommunicationScore { get; set; }
        public int ProblemSolvingScore { get; set; }
        public string? Comments { get; set; }

        // Expected values: "Hire", "Hold", "Reject"
        public string Recommendation { get; set; } = string.Empty;
    }
}