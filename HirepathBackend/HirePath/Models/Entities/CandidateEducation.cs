using HirePathAI.API.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateEducation : BaseEntity
    {
        [Required]
        public int CandidateProfileId { get; set; }

        [ForeignKey(nameof(CandidateProfileId))]
        public CandidateProfile? CandidateProfile { get; set; }

        // ============ EXISTING FIELDS ============
        [Required]
        [MaxLength(200)]
        public string Institute { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Qualification { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        [MaxLength(10)]
        public string? Grade { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // ============ NEW FIELDS ============
        [MaxLength(500)]
        public string? CertificateUrl { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? EducationLevel { get; set; }

        public decimal? GPA { get; set; }

        public decimal? Percentage { get; set; }

        public bool IsVerified { get; set; }

        [MaxLength(100)]
        public string? VerifiedBy { get; set; }
    }
}