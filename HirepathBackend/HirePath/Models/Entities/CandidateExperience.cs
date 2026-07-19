using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateExperience : BaseEntity
    {
        [Required]
        public int CandidateProfileId { get; set; }

        [ForeignKey(nameof(CandidateProfileId))]
        public CandidateProfile? CandidateProfile { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }  // Added this field

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }

        public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;  // Added this field
    }
}