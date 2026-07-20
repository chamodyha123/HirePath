using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Evaluation
{
    public class CreateEvaluationDto
    {
        [Range(1, int.MaxValue)]
        public int JobApplicationId { get; set; }

        [Range(typeof(decimal), "0", "100")]
        public decimal? ResumeScore { get; set; }

        [Range(typeof(decimal), "0", "100")]
        public decimal? AIScore { get; set; }
    }
}
