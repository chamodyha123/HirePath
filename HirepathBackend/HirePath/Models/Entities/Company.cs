using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Location { get; set; }


        // Company Approval
        public CompanyStatus Status { get; set; }
            = CompanyStatus.Pending;


        // Company Details
        public string? Industry { get; set; }

        public string? BusinessRegistrationNumber { get; set; }

        public string? CompanyEmail { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }


        // Company Representative
        public string? RepresentativeName { get; set; }

        public string? RepresentativeEmail { get; set; }


        // Platform Admin Review Information
        public DateTime? ApprovedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        public DateTime? SuspendedAt { get; set; }

        public string? RejectionReason { get; set; }

        public string? AdminNotes { get; set; }


        // Existing Relationships
        public ICollection<Department> Departments { get; set; }
            = new List<Department>();

        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();
    }
}