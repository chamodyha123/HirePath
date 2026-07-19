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

        // Kept because the recruiter module
        // may already use this field.
        [MaxLength(150)]
        public string? Location { get; set; }

        [Url]
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        public CompanyStatus Status { get; set; }
            = CompanyStatus.Pending;

        public ICollection<Department> Departments { get; set; }
            = new List<Department>();

        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();

        public ICollection<CompanyMember> Members { get; set; }
            = new List<CompanyMember>();

        public ICollection<CompanyInvitation> Invitations { get; set; }
            = new List<CompanyInvitation>();
    }
}