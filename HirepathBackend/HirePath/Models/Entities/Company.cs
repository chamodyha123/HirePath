using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Industry { get; set; }

        [MaxLength(100)]
        public string? BusinessRegistrationNumber { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Url]
        [MaxLength(250)]
        public string? Website { get; set; }

        [MaxLength(150)]
        public string? Location { get; set; }

        [Url]
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // Company Representative
        [MaxLength(150)]
        public string? RepresentativeName { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? RepresentativeEmail { get; set; }

        // Platform Admin approval
        public CompanyStatus Status { get; set; }
            = CompanyStatus.Pending;

        public DateTime? ApprovedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        public DateTime? SuspendedAt { get; set; }

        [MaxLength(1000)]
        public string? RejectionReason { get; set; }

        [MaxLength(1000)]
        public string? AdminNotes { get; set; }

        // Relationships
        public ICollection<Department> Departments { get; set; }
            = new List<Department>();

        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();

        // Recruiters / Hiring Managers working for this company
        public ICollection<User> Employees { get; set; }
            = new List<User>();

        public ICollection<CompanyMember> Members { get; set; }
            = new List<CompanyMember>();

        public ICollection<CompanyInvitation> Invitations { get; set; }
            = new List<CompanyInvitation>();
    }
}