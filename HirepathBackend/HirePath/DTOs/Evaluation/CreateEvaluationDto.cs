using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Evaluation
{
    // ResumeScore and AIScore can be supplied manually by a Hiring Manager,
    // or left null so the service falls back to the AI match score
    // already stored on the JobApplication.
    // InterviewScore is calculated server-side from InterviewFeedback.
    public class CreateEvaluationDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "JobApplicationId must be greater than 0.")]
        public int JobApplicationId { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "100",
            ErrorMessage = "ResumeScore must be between 0 and 100.")]
        public decimal? ResumeScore { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "100",
            ErrorMessage = "AIScore must be between 0 and 100.")]
        public decimal? AIScore { get; set; }
    }
}