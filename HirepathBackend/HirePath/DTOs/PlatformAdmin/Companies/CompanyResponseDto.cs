namespace HirePathAI.API.DTOs.PlatformAdmin.Companies
{
    public class CompanyResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Location { get; set; }

        public string? Industry { get; set; }

        public string? BusinessRegistrationNumber { get; set; }

        public string? CompanyEmail { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? RepresentativeName { get; set; }

        public string? RepresentativeEmail { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? ApprovedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        public DateTime? SuspendedAt { get; set; }

        public string? RejectionReason { get; set; }

        public string? AdminNotes { get; set; }
    }
}