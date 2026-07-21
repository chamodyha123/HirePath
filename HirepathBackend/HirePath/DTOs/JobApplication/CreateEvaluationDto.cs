using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class CreateEvaluationDto
    {
        [Required]
        public int ApplicationId { get; set; }

        [Range(0, 100)]
        public decimal? ResumeScore { get; set; }

        [Range(0, 100)]
        public decimal? AIScore { get; set; }

        [Range(0, 100)]
        public decimal? InterviewScore { get; set; }

        [Range(0, 100)]
        public decimal? HiringManagerScore { get; set; }

        public string? Comments { get; set; }
        public string? Recommendations { get; set; }

        public bool IsFinalized { get; set; }
    }

    public class EvaluationResponseDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int EvaluatorId { get; set; }
        public string? EvaluatorName { get; set; }
        public decimal? ResumeScore { get; set; }
        public decimal? AIScore { get; set; }
        public decimal? InterviewScore { get; set; }
        public decimal? HiringManagerScore { get; set; }
        public decimal? OverallScore { get; set; }
        public string? Comments { get; set; }
        public string? Recommendations { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime EvaluationDate { get; set; }

        // Additional info for display
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
    }
}