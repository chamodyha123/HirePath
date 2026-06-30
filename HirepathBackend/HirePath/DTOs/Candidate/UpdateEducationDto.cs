using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Candidate  
{
    public class UpdateEducationDto
    {
        [Required(ErrorMessage = "Institute name is required")]
        [StringLength(200, ErrorMessage = "Institute name cannot exceed 200 characters")]
        public required string Institute { get; set; }

        [Required(ErrorMessage = "Qualification is required")]
        [StringLength(100, ErrorMessage = "Qualification cannot exceed 100 characters")]
        public required string Qualification { get; set; }

        [StringLength(100, ErrorMessage = "Field of study cannot exceed 100 characters")]
        public string? FieldOfStudy { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }
}