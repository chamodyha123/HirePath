namespace HirePathAI.API.DTOs.Interview
{
    public class EvaluateInterviewDto
    {
        public int InterviewId { get; set; }
        public decimal Score { get; set; }
        public string? Feedback { get; set; }
    }
}