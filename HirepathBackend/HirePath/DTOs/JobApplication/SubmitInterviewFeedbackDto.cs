using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class SubmitInterviewFeedbackDto
    {
        [Required]
        public int InterviewId { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [Range(0, 100)]
        public decimal? TechnicalScore { get; set; }

        [Range(0, 100)]
        public decimal? CommunicationScore { get; set; }

        [Range(0, 100)]
        public decimal? ProblemSolvingScore { get; set; }

        [Range(0, 100)]
        public decimal? CulturalFitScore { get; set; }

        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }

        [Required]
        public HiringRecommendation Recommendation { get; set; }
    }

    public class InterviewFeedbackResponseDto
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public int ApplicationId { get; set; }
        public int EvaluatorId { get; set; }
        public string? EvaluatorName { get; set; }
        public decimal? TechnicalScore { get; set; }
        public decimal? CommunicationScore { get; set; }
        public decimal? ProblemSolvingScore { get; set; }
        public decimal? CulturalFitScore { get; set; }
        public decimal? OverallScore { get; set; }
        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public HiringRecommendation Recommendation { get; set; }
        public DateTime FeedbackDate { get; set; }
        public bool IsSubmitted { get; set; }
    }
}