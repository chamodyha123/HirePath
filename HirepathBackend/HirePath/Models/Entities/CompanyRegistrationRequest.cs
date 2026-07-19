using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CompanyRegistrationRequest : BaseEntity
    {
        // =========================
        // COMPANY INFORMATION
        // =========================

        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; }
            = string.Empty;

        [MaxLength(100)]
        public string? Industry { get; set; }

        [MaxLength(100)]
        public string? BusinessRegistrationNumber { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string CompanyEmail { get; set; }
            = string.Empty;

        [Phone]
        [MaxLength(30)]
        public string? CompanyPhone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [Url]
        [MaxLength(250)]
        public string? Website { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Url]
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // =========================
        // COMPANY REPRESENTATIVE
        // =========================

        [Required]
        [MaxLength(150)]
        public string RepresentativeName { get; set; }
            = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string RepresentativeEmail { get; set; }
            = string.Empty;

        [Phone]
        [MaxLength(30)]
        public string? RepresentativePhone { get; set; }

        // =========================
        // REQUEST STATUS
        // =========================

        public CompanyRegistrationStatus Status { get; set; }
            = CompanyRegistrationStatus.Pending;

        // =========================
        // PLATFORM ADMIN REVIEW
        // =========================

        [MaxLength(1000)]
        public string? ReviewNote { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByUserId { get; set; }

        public User? ReviewedByUser { get; set; }

        // =========================
        // APPROVED COMPANY
        // =========================

        public int? CreatedCompanyId { get; set; }

        public Company? CreatedCompany { get; set; }
    }
}