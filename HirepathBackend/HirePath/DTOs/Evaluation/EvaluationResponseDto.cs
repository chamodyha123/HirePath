namespace HirePathAI.API.DTOs.Evaluation
{
    public class EvaluationResponseDto
    {
        public int JobApplicationId { get; set; }
        public decimal ResumeScore { get; set; }
        public decimal AIScore { get; set; }
        public decimal InterviewScore { get; set; }
        public decimal OverallScore { get; set; }
        public int EvaluatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}