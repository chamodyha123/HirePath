namespace HirePathAI.API.DTOs.Evaluation
{
    // ResumeScore/AIScore can be supplied manually by a Hiring Manager,
    // or left null so the service falls back to the AI match score
    // already stored on the JobApplication (from the AI matching module).
    // InterviewScore is always computed server-side from InterviewFeedback.
    public class CreateEvaluationDto
    {
        public int JobApplicationId { get; set; }
        public decimal? ResumeScore { get; set; }
        public decimal? AIScore { get; set; }
    }
}