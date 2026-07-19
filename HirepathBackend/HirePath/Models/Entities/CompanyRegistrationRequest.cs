using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CompanyRegistrationRequest
    {
        public int Id { get; set; }


        // Company information
        public string CompanyName { get; set; }
            = string.Empty;

        public string? Industry { get; set; }

        public string? BusinessRegistrationNumber { get; set; }

        public string CompanyEmail { get; set; }
            = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }


        // Company representative
        public string RepresentativeName { get; set; }
            = string.Empty;

        public string RepresentativeEmail { get; set; }
            = string.Empty;


        // Request status
        public CompanyStatus Status { get; set; }
            = CompanyStatus.Pending;


        // Platform Admin review information
        public string? AdminNotes { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }


        // Approved company
        public int? CompanyId { get; set; }

        public Company? Company { get; set; }
    }
}