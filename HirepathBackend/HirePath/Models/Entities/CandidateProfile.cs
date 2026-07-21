using HirePathAI.API.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateProfile : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        // ============ BASIC INFORMATION ============
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(200)]
        public string? Headline { get; set; }

        public string? Summary { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsProfileComplete { get; set; }

        public DateTime? ProfileUpdatedAt { get; set; }

        // ============ PROFILE PICTURE ============
        public int? ProfilePictureId { get; set; }

        // REMOVE [ForeignKey] from here - it's already defined in ProfilePicture
        public ProfilePicture? ProfilePicture { get; set; }

        // ============ PERSONAL INFORMATION ============
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        // ============ PREFERENCES ============
        [MaxLength(50)]
        public string? PreferredWorkMode { get; set; }

        // ============ SOCIAL LINKS ============
        [MaxLength(200)]
        public string? GitHubUrl { get; set; }

        [MaxLength(200)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(200)]
        public string? PortfolioUrl { get; set; }

        // ============ ADDITIONAL ============
        [MaxLength(200)]
        public string? Languages { get; set; }

        // ============ NAVIGATION PROPERTIES ============
        public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
        public ICollection<CandidateEducation> Educations { get; set; } = new List<CandidateEducation>();
        public ICollection<CandidateExperience> Experiences { get; set; } = new List<CandidateExperience>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}